    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivadorPersonajeTrigger : MonoBehaviour
{
	public UnityEvent eventoActivarEnter;
	public UnityEvent eventoActivarExit;
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			eventoActivarEnter.Invoke();
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			eventoActivarExit.Invoke();
		}
	}
}
