using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BotonPorDefecto : MonoBehaviour
{

    public GameObject[] botones;
    
	// Update is called once per frame
	public void Iniciar(int cual)
    {
        EventSystem.current.SetSelectedGameObject(botones[cual]);
    }
}
