using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plumero : MonoBehaviour
{
    public static Plumero singleton;

    public Image plumeroImage;
    public Sprite plumaRoja, plumaAzul, plumaVerde;

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
                break;
            case Pluma.roja:
                plumeroImage.sprite = plumaRoja;
                break;
            case Pluma.verde:
                plumeroImage.sprite = plumaVerde;
                break;
            default:
                break;
        }
    }
}

public enum Pluma { azul, roja, verde }