using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvitionalMovement : MonoBehaviour
{
    public float velocity = 1;

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * velocity * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * velocity * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * velocity * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * velocity * Time.fixedDeltaTime;
        }
    }
}
