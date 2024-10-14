using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MarcoSangre : MonoBehaviour
{
    public Image imMarco;
    [Range(0, 1)]
    public float maximo = 0.5f;
    public float frecuencia = 20;
    public static MarcoSangre singleton;

    private void Awake()
    {
        singleton = this;
    }
    [ContextMenu("Iniciar")]
    public void Iniciar1Segundos()
	{
        IniciarEfecto(1);
	}

    public void IniciarEfecto(float tiempo)
    {
        StartCoroutine(EfectoSangre(tiempo));
        AudioManager.Instance.PlayOneShot(EventsManager.Instance.PlayerHurt, this.transform.position);
    }

    private IEnumerator EfectoSangre(float tiempo)
    {
        float tiempoTranscurrido = 0f;
        Color colorOriginal = imMarco.color;

        while (tiempoTranscurrido < tiempo)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Calcula el valor del alfa usando una función seno para suavidad
            float alfa = Mathf.Sin(tiempoTranscurrido * frecuencia) * 0.5f + 0.5f;
            alfa *= maximo;
            // Actualiza el color del `Image` con el nuevo valor alfa
            imMarco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alfa);

            yield return null;
        }

        // Asegura que al terminar el efecto, el alfa vuelva a 0
        imMarco.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0);
    }
}

