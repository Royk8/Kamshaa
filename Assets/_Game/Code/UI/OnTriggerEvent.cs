using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public int manyTimesLeft = 1;
    public UnityEvent onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (manyTimesLeft <= 0) return;
        onTriggerEnter?.Invoke();
        manyTimesLeft--;
    }
}
