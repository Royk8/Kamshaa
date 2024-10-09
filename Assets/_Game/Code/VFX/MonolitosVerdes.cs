using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonolitosVerdes : MonoBehaviour
{
    public Transform monolito;  // El objeto a mover
    public Vector3 posicionInicial;  // Posici�n local inicial del objeto
    public Vector3 posFinal;  // Posici�n local final del objeto
    public float tiempoCambioPosicion = 2f;  // Tiempo total de la transici�n
    public float magnitudShake = 0.1f;  // Intensidad de la vibraci�n
    public GameObject[] objetosInvertirActivo;

    private bool enMovimiento = false;
    bool yaTermino = false;

    private void Start()
    {
        // Inicializa la posici�n local del monolito en la posici�n inicial
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
            // Calcula el progreso de la interpolaci�n
            float t = tiempoTranscurrido / tiempoCambioPosicion;

            // Interpola la posici�n local del monolito entre la posici�n inicial y final
            Vector3 posicionInterpolada = Vector3.Lerp(posicionInicialMonolito, posFinal, t);

            // A�ade vibraci�n a la posici�n interpolada en el espacio local
            Vector3 shakeOffset = Random.insideUnitSphere * magnitudShake;
            monolito.localPosition = posicionInterpolada + shakeOffset;

            // Incrementa el tiempo transcurrido y espera al siguiente frame
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Asegura que el monolito termina en la posici�n final exacta
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
            // Calcula el progreso de la interpolaci�n
            float t = tiempoTranscurrido / tiempoCambioPosicion;

            // Interpola la posici�n local del monolito entre la posici�n inicial y final
            Vector3 posicionInterpolada = Vector3.Lerp(posicionInicialMonolito, posicionInicial, t);

            // A�ade vibraci�n a la posici�n interpolada en el espacio local
            Vector3 shakeOffset = Random.insideUnitSphere * magnitudShake;
            monolito.localPosition = posicionInterpolada + shakeOffset;

            // Incrementa el tiempo transcurrido y espera al siguiente frame
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Asegura que el monolito termina en la posici�n final exacta
        monolito.localPosition = posicionInicial;
        enMovimiento = false;
        yaTermino = true;
    }
}
