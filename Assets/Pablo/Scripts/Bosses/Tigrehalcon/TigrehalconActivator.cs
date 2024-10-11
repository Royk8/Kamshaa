using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigrehalconActivator : MonoBehaviour
{
    public Tigrehalcon tigrehalcon;
    public Collider _collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tigrehalcon.enabled = true;
            _collider.enabled = true;
        }
    }
}
