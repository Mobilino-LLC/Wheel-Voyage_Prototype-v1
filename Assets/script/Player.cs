using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Globalization;
using Newtonsoft.Json.Linq;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public float uprightThreshold = 0.5f;
    public float uprightSpeed = 2.0f;
    public string command = "prctInit";

    private Rigidbody rb;
    private SerialPort serialPort = new SerialPort("COM5", 9600);
    private float leftWheelValue;
    private float rightWheelValue;

    private Text m1Text;
    private Text m2Text;
    private InputField serialInput;
    private Button serialBtn;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        m1Text = GameObject.Find("M1VAL").GetComponent<Text>();
        m2Text = GameObject.Find("M2VAL").GetComponent<Text>();
        serialInput = GameObject.Find("SerialInput").GetComponent<InputField>();
        serialBtn = GameObject.Find("SerialBtn").GetComponent<Button>();

        serialBtn.onClick.AddListener(OnSerialBtnClick);

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
            catch (System.Exception e) {
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

    bool IsTippedOver() {
        return Mathf.Abs(transform.up.z) < uprightThreshold;
    }

    void Upright() {
        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * uprightSpeed);
    }

    void OnSerialBtnClick() {
        string inputValue = serialInput.text;
        // string inputValue = command;

        if (!string.IsNullOrEmpty(inputValue)) {
            sendDataSerial(inputValue);
        }
        else {
            Debug.LogWarning("Input value is empty.");
        }

        serialInput.text = "";
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
        try {
            JObject json = JObject.Parse(dataString);
            leftWheelValue = json.ContainsKey("M1VAL") ? (float)json["M1VAL"] : 0.0f;
            rightWheelValue = json.ContainsKey("M2VAL") ? (float)json["M2VAL"] : 0.0f;
            string receivedData = json.ContainsKey("Received") ? (string)json["Received"] : "";

            if (!string.IsNullOrEmpty(receivedData))
                Debug.Log("Received: " + receivedData);

            m1Text.text = "M1: " + leftWheelValue.ToString();
            m2Text.text = "M2: " + rightWheelValue.ToString();

            if (leftWheelValue <= 3)
                leftWheelValue = 0;
            else
                leftWheelValue -= 3;

            if (rightWheelValue <= 3)
                rightWheelValue = 0;
            else
                rightWheelValue -= 3;
        }
        catch (System.Exception e) {
            Debug.LogWarning("Failed to parse serial data: " + e.Message);
        }
    }

    void OnApplicationQuit() {
        if (serialPort.IsOpen) {
            serialPort.Close();
        }
    }
}
