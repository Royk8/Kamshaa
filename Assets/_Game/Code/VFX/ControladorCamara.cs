using UnityEngine;

public class ControladorCamara : MonoBehaviour
{
    public Transform objetivo;  // El objetivo (personaje) que seguirá la cámara
    public float velocidadSuavizada = 0.125f;  // Velocidad de seguimiento suave
    private Vector3 desplazamientoInicial;  // Distancia inicial de la cámara al objetivo

    private Vector3 posicionOriginal;  // Posición original para el shake
    private bool estaTemblando = false;  // Control del estado de shake
    private float duracionTemblor = 0f;  // Duración del shake
    private float magnitudTemblor = 0f;  // Intensidad del shake

    public Camera[] camaras;
    public float apertura;
    private float aperturaInicial;
    public static ControladorCamara singleton;

    private void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        if (objetivo != null)
        {
            desplazamientoInicial = transform.position - objetivo.position;
        }

        posicionOriginal = transform.position;
        aperturaInicial = camaras[0].orthographicSize;
        apertura = aperturaInicial;
    }

    public void CambiarApertura(float cuanto)
    {
        apertura = cuanto;
    }

    public void RestaurarApertura()
    {
        apertura = aperturaInicial;
    }

    void LateUpdate()
    {
        if (objetivo != null)
        {
            Vector3 posicionDeseada = objetivo.position + desplazamientoInicial;
            if (posicionDeseada.y < 0) posicionDeseada.y = 3;
            Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizada);

            if (estaTemblando && duracionTemblor > 0)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * magnitudTemblor;
                transform.position = posicionSuavizada + shakeOffset;
                duracionTemblor -= Time.deltaTime;
            }
            else
            {
                estaTemblando = false;
                transform.position = posicionSuavizada;
            }
        }
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(camaras[0].orthographicSize - apertura) > 0.005f)
        {
            for (int i = 0; i < camaras.Length; i++)
            {
                camaras[i].orthographicSize = Mathf.Lerp(camaras[i].orthographicSize, apertura, 0.1f);
            }
        }
    }

    public void IniciarTemblor(float duracion, float magnitud)
    {
        duracionTemblor = duracion;
        magnitudTemblor = magnitud;
        estaTemblando = true;
    }
}
