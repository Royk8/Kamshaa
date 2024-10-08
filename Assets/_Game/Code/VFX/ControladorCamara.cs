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

    void Start()
    {
        // Calcula y guarda la distancia inicial de la cámara al objetivo
        if (objetivo != null)
        {
            desplazamientoInicial = transform.position - objetivo.position;
        }

        // Guarda la posición original de la cámara
        posicionOriginal = transform.position;
    }

    void LateUpdate()
    {
        // Si hay un objetivo, realiza el seguimiento
        if (objetivo != null)
        {
            Vector3 posicionDeseada = objetivo.position + desplazamientoInicial;  // Mantén la distancia original
            if (posicionDeseada.y < 0) posicionDeseada.y = 3;
            Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizada);  // Movimiento suavizado
            transform.position = posicionSuavizada;
        }

        // Si está temblando, aplica el shake
        if (estaTemblando)
        {
            if (duracionTemblor > 0)
            {
                transform.position = posicionOriginal + Random.insideUnitSphere * magnitudTemblor;  // Agrega el shake

                // Decrementa la duración del shake
                duracionTemblor -= Time.deltaTime;
            }
            else
            {
                // Termina el shake y regresa la cámara a su posición original
                estaTemblando = false;
                transform.position = posicionOriginal;
            }
        }
    }

    // Método público para iniciar el temblor de la cámara
    public void IniciarTemblor(float duracion, float magnitud)
    {
        posicionOriginal = transform.position;  // Guarda la posición inicial antes del shake
        duracionTemblor = duracion;  // Configura la duración del temblor
        magnitudTemblor = magnitud;  // Configura la magnitud del temblor
        estaTemblando = true;  // Inicia el temblor
    }
}
