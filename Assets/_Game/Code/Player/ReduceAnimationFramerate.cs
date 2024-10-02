using UnityEngine;

public class ReduceAnimationFramerate : MonoBehaviour
{
    public Animator animator;                // Referencia al Animator
    public float targetFramerate = 15f;      // Framerate objetivo para la animación (por ejemplo, 15 FPS)

    private float frameTime;                 // Tiempo que dura un frame basado en el framerate objetivo
    private float timeSinceLastFrame;        // Acumulador de tiempo para controlar el frame skipping

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Calcular la duración de cada frame según el framerate objetivo
        frameTime = 1f / targetFramerate;
    }

    void Update()
    {
        // Acumular el tiempo transcurrido en el juego
        timeSinceLastFrame += Time.deltaTime;

        // Si ha pasado suficiente tiempo para actualizar un frame según el framerate objetivo
        if (timeSinceLastFrame >= frameTime)
        {
            // Forzar al Animator a avanzar un frame equivalente al frameTime
            animator.Update(timeSinceLastFrame);

            // Resetear el acumulador para el siguiente frame
            timeSinceLastFrame = 0f;
        }
    }
}
