using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 10;
    public float rotateSpeed = 0.3f;

    void Update()
    {
        movement();
    }
    void movement()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(0, -rotateSpeed, 0);
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(0, rotateSpeed, 0);
        if (Input.GetKey(KeyCode.UpArrow))
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.DownArrow))
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
