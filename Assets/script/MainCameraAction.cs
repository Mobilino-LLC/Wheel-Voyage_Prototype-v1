using UnityEngine;

public class MainCameraAction : MonoBehaviour
{
    public Transform target;  
    public Vector3 offset = new Vector3(0.0f, 3.0f, -6.0f);
    public float positionSmoothSpeed = 0.125f; 
    public float rotationSmoothSpeed = 5.0f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + target.TransformDirection(offset);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed);
            transform.position = smoothedPosition;

            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position + target.forward * 2, Vector3.up);
            Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
            transform.rotation = smoothedRotation;
        }
    }
}