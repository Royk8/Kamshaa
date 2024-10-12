using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlEfectosGato : MonoBehaviour
{
    public Animator animaiciones;

	[ContextMenu("Iniciar")]
    public void IniciarAtaque()
	{
		animaiciones.SetBool("atacando", true);
	}
	[ContextMenu("Terminar")]
	public void TerminarAtaque()
	{
		animaiciones.SetBool("atacando", false);
	}
}
