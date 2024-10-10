using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsControl : MonoBehaviour
{
    public Animator animator;
    [SerializeField]
    private bool enPiso;
    public bool autoCalcularVelocidad;
    public VFXControlPersonaje vfx;

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
        Vector3 aPos;
		while (autoCalcularVelocidad)
		{
            yield return new WaitForSeconds(0.1f);
            aPos = transform.position;
            aPos.y = 0;
            CambiarVelocidad((aPos - bPos).sqrMagnitude*3);
            bPos = transform.position;
            bPos.y = 0;
		}
	}

	public void Dash()
	{
        animator.SetTrigger("dash");
		if (vfx!=null)
		{
            vfx.Dash();
		}
    }
    public void DobleSalto()
    {
        animator.SetTrigger("resalto");
        if (vfx != null)
        {
            vfx.DobleSalto();
        }
    }
    public void Disparar()
    {
        animator.SetTrigger("poder");
    }
    [ContextMenu("Marear")]
    public void IniciarMareo()
    {
        animator.SetBool("mareado", true);
        vfx.SetMareado(true);
    }
    [ContextMenu("Desmarear")]
    public void FinalizarMareo()
    {
        animator.SetBool("mareado", false);
        vfx.SetMareado(false);
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
