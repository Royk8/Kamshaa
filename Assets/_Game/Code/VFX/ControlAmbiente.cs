using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ControlAmbiente : MonoBehaviour
{
    public Material materialGeneral;
    [Range(0, 1)]
    public float r, g, b;

    private Coroutine corutinaR, corutinaG, corutinaB, corutinaCompleta;
    public static ControlAmbiente singleton;
    public UnityEvent ganaColor;
    public UnityEvent ganaTodosColores;

	private void Awake()
	{
        singleton = this;
	}
	private void Start()
    {
        ActualizarMaterial();
    }

    public void ActualizarMaterial()
    {
        materialGeneral.SetFloat("_r", r);
        materialGeneral.SetFloat("_g", g);
        materialGeneral.SetFloat("_b", b);
    }

    [ContextMenu("LlenarRojo")]
    public void LlenarRojo()
    {
        if (corutinaR != null) StopCoroutine(corutinaR);
        ganaColor.Invoke();
        corutinaR = StartCoroutine(LlenarVariableSuave("r"));
    }

    [ContextMenu("LlenarVerde")]
    public void LlenarVerde()
    {
        if (corutinaG != null) StopCoroutine(corutinaG);
        ganaColor.Invoke();
        corutinaG = StartCoroutine(LlenarVariableSuave("g"));
    }

    [ContextMenu("LlenarAzul")]
    public void LlenarAzul()
    {
        if (corutinaB != null) StopCoroutine(corutinaB);
        ganaColor.Invoke();
        corutinaB = StartCoroutine(LlenarVariableSuave("b"));
    }

    private IEnumerator LlenarVariableSuave(string color)
    {
        float tiempoTotal = 2f;
        float valorInicial = 0;
        float objetivo = 0.7f;
        float tiempoTranscurrido = 0f;

        if (color == "r") valorInicial = r;
        if (color == "g") valorInicial = g;
        if (color == "b") valorInicial = b;

        while (tiempoTranscurrido < tiempoTotal)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / tiempoTotal;
            float nuevoValor = Mathf.Lerp(valorInicial, objetivo, t);

            switch (color)
            {
                case "r": r = nuevoValor; break;
                case "g": g = nuevoValor; break;
                case "b": b = nuevoValor; break;
            }

            ActualizarMaterial();

            // Verifica si todas las variables han llegado a 0.7
            if (r >= 0.7f && g >= 0.7f && b >= 0.7f && corutinaCompleta == null)
            {
                corutinaCompleta = StartCoroutine(LlenarCompleto());
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator LlenarCompleto()
    {
        float tiempoTotal = 2f;
        float tiempoTranscurrido = 0f;

        float rInicial = r;
        float gInicial = g;
        float bInicial = b;

        ganaTodosColores.Invoke();
        while (tiempoTranscurrido < tiempoTotal)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / tiempoTotal;

            r = Mathf.Lerp(rInicial, 1f, t);
            g = Mathf.Lerp(gInicial, 1f, t);
            b = Mathf.Lerp(bInicial, 1f, t);

            ActualizarMaterial();

            yield return null;
        }

        // Asegura que los valores estén exactamente en 1
        r = g = b = 1f;
        ActualizarMaterial();
        corutinaCompleta = null;
    }
}
