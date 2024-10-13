using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class PersonajeTriggerea : MonoBehaviour
{
    public UnityEvent entraTigger;
    public UnityEvent saleTigger;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			entraTigger.Invoke();
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			entraTigger.Invoke();
		}
	}


}
