using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXControlPersonaje : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public Movement movimiento;
    public ControladorCamara conCamara;
    public GameObject particulasDobleSalto;

    void Start()
    {
        
    }

    
    public void Dash()
	{
        StartCoroutine(Dashear());
		if (conCamara!= null)
		{
            conCamara.IniciarTemblor(0.1f, 0.2f);
		}
	}

    IEnumerator Dashear()
	{
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(movimiento.dashTime);
        trailRenderer.emitting = false;

    }

    public void DobleSalto()
	{
        Instantiate(particulasDobleSalto, transform.position + Vector3.up*1.5f, Quaternion.identity);
	}
}
