using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Checkpointer : MonoBehaviour
{
    private Vector3 restartPosition;
    private Movement movement;

    private void Start()
    {
        restartPosition = transform.position;
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        RestartPosition();
    }

    public void RestartPosition()
    {
        if (transform.position.y < -20)
        {
            movement.SetPosition(restartPosition);
            StartCoroutine(movement.StopGravityCoroutine(0.1f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint");
            restartPosition = other.transform.position;
        }
    }
}
