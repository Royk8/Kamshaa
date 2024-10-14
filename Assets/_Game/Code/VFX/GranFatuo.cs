using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GranFatuo : MonoBehaviour
{
    public GameObject iconoMinimapa;
    public ParticleSystem particulas;

    public UnityEvent evento;


    public void Activar()
	{
        particulas.Stop();
        iconoMinimapa.SetActive(false);
        evento.Invoke();
	}

}
