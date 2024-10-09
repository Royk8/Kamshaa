using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senos : MonoBehaviour
{
    public Vector3 amplitud;
    public float frecuencia = 0.2f;

    Vector3 posInicial;
    void Start()
    {
        posInicial = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = posInicial +  Mathf.Sin(Time.time * frecuencia) * amplitud;
    }
}
