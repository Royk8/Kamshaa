using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalIntermitente : MonoBehaviour
{
    public float frecuencia = 20;
    private DecalProjector decalProjector;
    private float tiempo = 0f;
    public float maximo = 0.5f;

    private void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
        if (decalProjector == null)
        {
            Debug.LogError("No se encontró un Decal Projector en el objeto.");
        }
    }

    private void Update()
    {
        if (decalProjector == null) return;

        // Calcula el nuevo valor de opacidad usando la función seno para suavidad
        tiempo += Time.deltaTime * frecuencia;
        float opacity = Mathf.Sin(tiempo) * 0.5f + 0.5f; // Oscila entre 0 y 1

        // Actualiza la opacidad del Decal Projector
        decalProjector.fadeFactor = opacity*maximo;
    }
}
