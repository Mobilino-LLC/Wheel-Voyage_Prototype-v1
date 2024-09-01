using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Globalization;
using Newtonsoft.Json.Linq;

public class Player : MonoBehaviour {
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public float uprightThreshold = 0.5f;
    public float uprightSpeed = 2.0f;

    private Rigidbody rb;
    private SerialPort serialPort = new SerialPort("COM5", 9600);
    private float leftWheelValue;
    private float rightWheelValue;

    private Text m1Text;
    private Text m2Text;
    private InputField serialInput;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        m1Text = GameObject.Find("M1VAL").GetComponent<Text>();
        m2Text = GameObject.Find("M2VAL").GetComponent<Text>();
        serialInput = GameObject.Find("SerialInput").GetComponent<InputField>();
        serialInput.onEndEdit.AddListener(OnSerialInputEndEdit);

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null) {
            Vector3 scale = boxCollider.transform.localScale;
            if (scale.x < 0 || scale.y < 0 || scale.z < 0) {
                Debug.LogWarning("BoxCollider scale is negative. Setting to positive.");
                scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
                boxCollider.transform.localScale = scale;
            }
        }
    }

    void Start() {
        serialPort.Open();
        serialPort.ReadTimeout = 100;
    }

    void Update() {
        if (serialPort.IsOpen) {
            try {
                string dataString = serialPort.ReadLine();
                ParseSerialData(dataString);
            }
            catch (System.Exception) {
                // catch (System.Exception e) {
                // Debug.LogWarning("Failed to read serial data: " + e.Message);
            }
        }
    }

    void FixedUpdate() {
        Vector3 movement = new Vector3(0, 0, -(leftWheelValue + rightWheelValue) / 2 * speed * Time.deltaTime);
        transform.Translate(movement);

        float rotation = (rightWheelValue - leftWheelValue) * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        if (IsTippedOver()) {
            Upright();
        }
    }

    void OnSerialInputEndEdit(string inputValue) {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (!string.IsNullOrEmpty(inputValue)) {
                sendDataSerial(inputValue);
            }

            serialInput.text = "";
        }
    }

    bool IsTippedOver() {
        return Mathf.Abs(transform.up.z) < uprightThreshold;
    }

    void Upright() {
        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * uprightSpeed);
    }

    void sendDataSerial(string sendData) {
        if (serialPort.IsOpen && !string.IsNullOrEmpty(sendData)) {
            try {
                serialPort.Write(sendData);
                Debug.Log("Sent to serial: " + sendData);
            }
            catch (System.Exception e) {
                Debug.LogError("Failed to send data to serial port: " + e.Message);
            }
        }
        else {
            Debug.LogWarning("Serial port not open or input value is empty.");
        }
    }

    void ParseSerialData(string dataString) {
        int backward_val = 1;
        int forward_val = 5;
        try {
            JObject json = JObject.Parse(dataString);
            leftWheelValue = json.ContainsKey("M1VAL") ? (float)json["M1VAL"] : 0.0f;
            rightWheelValue = json.ContainsKey("M2VAL") ? (float)json["M2VAL"] : 0.0f;
            string receivedData = json.ContainsKey("Received") ? (string)json["Received"] : "";

            if (!string.IsNullOrEmpty(receivedData))
                Debug.Log("Received: " + receivedData);

            m1Text.text = "M1: " + leftWheelValue.ToString();
            m2Text.text = "M2: " + rightWheelValue.ToString();

            if (leftWheelValue <= backward_val) {
                leftWheelValue -= backward_val;
            } else if (leftWheelValue > backward_val && leftWheelValue < forward_val) {
                leftWheelValue = 0;
            } else {
                leftWheelValue -= forward_val;
            }

            if (rightWheelValue <= backward_val) {
                rightWheelValue -= backward_val;
            } else if (rightWheelValue > backward_val && rightWheelValue < forward_val) {
                rightWheelValue = 0;
            } else {
                rightWheelValue -= forward_val;
            }
        }
        catch (System.Exception e) {
            Debug.LogWarning("Failed to parse serial data: " + e.Message);
        }
    }

    void OnApplicationQuit() {
        if (serialPort.IsOpen)
            serialPort.Close();
    }
}
