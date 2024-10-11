using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlasPos : MonoBehaviour
{
    public Vector3[] angulos;
    public float periodo = 0.2f;
    int i = 0;
    
    void Start()
    {
        StartCoroutine(UpdateMorion());
    }

    // Update is called once per frame
    IEnumerator UpdateMorion()
    {
		while (true)
		{
            yield return new WaitForSeconds(periodo);
            transform.localEulerAngles = angulos[i];
            i = (i + 1) % angulos.Length;
		}
    }
}
