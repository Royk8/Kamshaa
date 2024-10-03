using UnityEngine;

public class ControladorCamara : MonoBehaviour
{
    public Transform objetivo;  // El objetivo (personaje) que seguir� la c�mara
    public float velocidadSuavizada = 0.125f;  // Velocidad de seguimiento suave
    private Vector3 desplazamientoInicial;  // Distancia inicial de la c�mara al objetivo

    private Vector3 posicionOriginal;  // Posici�n original para el shake
    private bool estaTemblando = false;  // Control del estado de shake
    private float duracionTemblor = 0f;  // Duraci�n del shake
    private float magnitudTemblor = 0f;  // Intensidad del shake

    void Start()
    {
        // Calcula y guarda la distancia inicial de la c�mara al objetivo
        if (objetivo != null)
        {
            desplazamientoInicial = transform.position - objetivo.position;
        }

        // Guarda la posici�n original de la c�mara
        posicionOriginal = transform.position;
    }

    void LateUpdate()
    {
        // Si hay un objetivo, realiza el seguimiento
        if (objetivo != null)
        {
            Vector3 posicionDeseada = objetivo.position + desplazamientoInicial;  // Mant�n la distancia original
            Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizada);  // Movimiento suavizado
            transform.position = posicionSuavizada;
        }

        // Si est� temblando, aplica el shake
        if (estaTemblando)
        {
            if (duracionTemblor > 0)
            {
                transform.position = posicionOriginal + Random.insideUnitSphere * magnitudTemblor;  // Agrega el shake

                // Decrementa la duraci�n del shake
                duracionTemblor -= Time.deltaTime;
            }
            else
            {
                // Termina el shake y regresa la c�mara a su posici�n original
                estaTemblando = false;
                transform.position = posicionOriginal;
            }
        }
    }

    // M�todo p�blico para iniciar el temblor de la c�mara
    public void IniciarTemblor(float duracion, float magnitud)
    {
        posicionOriginal = transform.position;  // Guarda la posici�n inicial antes del shake
        duracionTemblor = duracion;  // Configura la duraci�n del temblor
        magnitudTemblor = magnitud;  // Configura la magnitud del temblor
        estaTemblando = true;  // Inicia el temblor
    }
}
