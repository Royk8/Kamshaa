using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonolitosVerdes : MonoBehaviour
{
    public Transform monolito;  // El objeto a mover
    public Vector3 posicionInicial;  // Posición local inicial del objeto
    public Vector3 posFinal;  // Posición local final del objeto
    public float tiempoCambioPosicion = 2f;  // Tiempo total de la transición
    public float magnitudShake = 0.1f;  // Intensidad de la vibración
    public GameObject[] objetosInvertirActivo;

    private bool enMovimiento = false;
    bool yaTermino = false;

    private void Start()
    {
        // Inicializa la posición local del monolito en la posición inicial
        if (monolito != null)
        {
            monolito.localPosition = posicionInicial;
        }
    }

    [ContextMenu("MoverConVibracion")]
    public void MoverConVibracion()
    {
        if (!enMovimiento && monolito != null && !yaTermino)
        {
            StartCoroutine(MoverYVibrar());
            ControladorCamara.singleton.IniciarTemblor(1.5f, 0.05f);
        }
    }
    [ContextMenu("VolverConVibracion")]
    public void VolverConVibracion()
    {
        if (!enMovimiento && monolito != null && yaTermino)
        {
            StartCoroutine(MoverYVibrar2());
            ControladorCamara.singleton.IniciarTemblor(1.5f, 0.05f);
        }
    }

    [ContextMenu("Invertir")]
    public void Finalizar()
    {
		for (int i = 0; i < objetosInvertirActivo.Length; i++)
		{
            objetosInvertirActivo[i].SetActive(!objetosInvertirActivo[i].activeSelf);
		}
        this.enabled = false;
    }

    private IEnumerator MoverYVibrar()
    {
        enMovimiento = true;
        Vector3 posicionInicialMonolito = monolito.localPosition;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoCambioPosicion)
        {
            // Calcula el progreso de la interpolación
            float t = tiempoTranscurrido / tiempoCambioPosicion;

            // Interpola la posición local del monolito entre la posición inicial y final
            Vector3 posicionInterpolada = Vector3.Lerp(posicionInicialMonolito, posFinal, t);

            // Añade vibración a la posición interpolada en el espacio local
            Vector3 shakeOffset = Random.insideUnitSphere * magnitudShake;
            monolito.localPosition = posicionInterpolada + shakeOffset;

            // Incrementa el tiempo transcurrido y espera al siguiente frame
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Asegura que el monolito termina en la posición final exacta
        monolito.localPosition = posFinal;
        enMovimiento = false;
        yaTermino = true;
    }


    private IEnumerator MoverYVibrar2()
    {
        enMovimiento = true;
        Vector3 posicionInicialMonolito = monolito.localPosition;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoCambioPosicion)
        {
            // Calcula el progreso de la interpolación
            float t = tiempoTranscurrido / tiempoCambioPosicion;

            // Interpola la posición local del monolito entre la posición inicial y final
            Vector3 posicionInterpolada = Vector3.Lerp(posicionInicialMonolito, posicionInicial, t);

            // Añade vibración a la posición interpolada en el espacio local
            Vector3 shakeOffset = Random.insideUnitSphere * magnitudShake;
            monolito.localPosition = posicionInterpolada + shakeOffset;

            // Incrementa el tiempo transcurrido y espera al siguiente frame
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Asegura que el monolito termina en la posición final exacta
        monolito.localPosition = posicionInicial;
        enMovimiento = false;
        yaTermino = true;
    }
}
