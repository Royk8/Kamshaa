using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plumero : MonoBehaviour
{
    public static Plumero singleton;

    public Image plumeroImage;
    public Sprite plumaRoja, plumaAzul, plumaVerde;
    public GameObject plumaRojaGO, plumaAzulGO, plumaVerdeGO;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    public void AdquirirPluma(Pluma pluma)
    {
        switch (pluma)
        {
            case Pluma.azul:
                plumeroImage.sprite = plumaAzul;
                plumaAzulGO.SetActive(true);
                break;
            case Pluma.roja:
                plumeroImage.sprite = plumaRoja;
                plumaRojaGO.SetActive(true);
                break;
            case Pluma.verde:
                plumeroImage.sprite = plumaVerde;
                plumaVerdeGO.SetActive(true);
                break;
            default:
                break;
        }
    }
}

public enum Pluma { azul, roja, verde }