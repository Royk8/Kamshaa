using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Monolito : MonoBehaviour
{
    public GameObject gmBueno;
    public GameObject gmFracturado;
    public Rigidbody[] rbs;
    public UnityEvent eventoDestruirMonolito;

    [ContextMenu("Romper")]
    public void Romper()
	{
        AudioManager.Instance.PlayOneShot(EventsManager.Instance.Monolito, this.transform.position);
        ControladorCamara.singleton.IniciarTemblor(0.5f, 0.2f);
        gmBueno.SetActive(false);
        gmFracturado.SetActive(true);
		for (int i = 0; i < rbs.Length; i++)
		{
            //rbs[i].angularVelocity=(Vector3.one * (Random.Range(-10000, 10000)));
            rbs[i].AddForce(Random.Range(-120, 120), Random.Range(-120, 120), Random.Range(-120, 120));
		}
        eventoDestruirMonolito.Invoke();
	}
}
