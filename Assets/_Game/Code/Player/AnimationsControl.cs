using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsControl : MonoBehaviour
{
    public Animator animator;
    [SerializeField]
    private bool enPiso;
    public bool autoCalcularVelocidad;

    Vector3 bPos;

	private void Start()
	{
        bPos = transform.position;
		if (autoCalcularVelocidad)
		{
            StartCoroutine(RecalcularMovimiento());
		}
	}

    IEnumerator RecalcularMovimiento()
	{
		while (autoCalcularVelocidad)
		{
            yield return new WaitForSeconds(0.2f);
            CambiarVelocidad((transform.position - bPos).sqrMagnitude);
            bPos = transform.position;
		}
	}

	public void Dash()
	{
        animator.SetTrigger("dash");
    }
    public void DobleSalto()
    {
        animator.SetTrigger("resalto");
    }
    public void Disparar()
    {
        animator.SetTrigger("poder");
    }
    public void CambiarVelocidad(float v)
	{
        animator.SetFloat("velocidad", v);
    }
    public void CambiarEnPiso(bool _enPiso)
    {
        enPiso = _enPiso;
        animator.SetBool("piso", enPiso);
    }
}
