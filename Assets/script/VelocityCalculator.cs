using UnityEngine;
using UnityEngine.UI;

public class CarSpeedometer : MonoBehaviour
{
    public Rigidbody carRigidbody; // 자동차의 Rigidbody 컴포넌트
    public Text speedText; // 속도를 표시할 UI 텍스트 컴포넌트

    void Start()
    {
        if (carRigidbody == null)
        {
            carRigidbody = GetComponent<Rigidbody>();
            if (carRigidbody == null)
            {
                Debug.LogError("Rigidbody가 할당되지 않았습니다.");
            }
        }

        if (speedText == null)
        {
            Debug.LogError("SpeedText가 할당되지 않았습니다.");
        }
    }

    void Update()
    {
        if (carRigidbody != null && speedText != null)
        {
            float speed = carRigidbody.velocity.magnitude * 3.6f; // m/s를 km/h로 변환
            speedText.text = "Speed: " + speed.ToString("F2") + " km/h";
        }
    }
}