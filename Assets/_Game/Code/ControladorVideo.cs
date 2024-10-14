using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class ControladorVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;        // Asigna el VideoPlayer en el Inspector
    public string nombreEscenaSiguiente;   // Nombre de la escena a la que se quiere cambiar
    public Image overlayImage;             // Asigna el Image en el Inspector
    public float duracionTransicion = 1f;  // Duración de la transición de fade-out
    public InputActionProperty botonOmitir;

    private bool omitirActivado = false;

    float tiempo;
    private void Start()
    {
        // Comienza la reproducción del video y detecta el final
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += CambiarEscenaAlFinalizarVideo;
        }
        botonOmitir.action.Enable();
        // Asegura que el overlay esté completamente transparente al inicio
        overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, 0);
    }

	private void Update()
	{
        tiempo += Time.deltaTime;
        if (botonOmitir.action.ReadValue<float>() > 0.8f)
        {
            OmitirDelay();
        }
    }

    public void OmitirDelay()
	{
        if (tiempo > 10)
        {
            Omitir();
        }
    }

	// Método para omitir el video con transición
	[ContextMenu("Omitir")]
    public void Omitir()
    {
        if (!omitirActivado)
        {
            omitirActivado = true;
            StartCoroutine(TransicionOmitir());
        }
    }

    // Al finalizar el video, cambiar de escena
    private void CambiarEscenaAlFinalizarVideo(VideoPlayer vp)
    {
        if (!omitirActivado)
        {
            SceneManager.LoadScene(nombreEscenaSiguiente);
        }
    }

    private IEnumerator TransicionOmitir()
    {
        float tiempo = 0f;
        Color colorInicial = overlayImage.color;
        Color colorFinal = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 1f);

        // Interpolación del alpha de 0 a 1 en duracionTransicion
        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            overlayImage.color = Color.Lerp(colorInicial, colorFinal, tiempo / duracionTransicion);
            yield return null;
        }

        // Cargar la siguiente escena
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}
