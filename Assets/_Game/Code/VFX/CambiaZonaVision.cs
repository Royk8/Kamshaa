using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambiaZonaVision : MonoBehaviour
{
    public ControladorCamara conCamara;
    public float nuevaApertura;

    public void AbrirEscena()
	{
        conCamara.CambiarApertura(nuevaApertura);
    }
    public void CerrarEscena()
    {
        conCamara.RestaurarApertura();
    }
}
