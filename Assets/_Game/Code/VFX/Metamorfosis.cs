using System.Collections;
using UnityEngine;

public class Metamorfosis : MonoBehaviour
{
    public Renderer[] mallas;
    public float tiempoTransición = 2f;
    public GameObject particulas;

    // Variables para el efecto de stun
    public float frecuenciaStun = 0.1f;
    public float tiempoStun = 0.5f;


    private void Start()
    {
        // Inicializa la propiedad "_tiempo" del material en -0.1
        foreach (Renderer renderer in mallas)
        {
            if (renderer != null)
            {
                renderer.material.SetFloat("_tiempo", -0.1f);
            }
        }
    }

    [ContextMenu("Iniciar Transición")]
    public void IniciarTransicion()
    {
        StartCoroutine(TransicionarMateriales());
    }

    private IEnumerator TransicionarMateriales()
    {
        float tiempoInicial = -0.1f;
        float tiempoFinal = 1.1f;
        float tiempoTranscurrido = 0f;

        // Instancia las partículas al final de la transición
        if (particulas != null)
        {
            Instantiate(particulas, transform.position + Vector3.up * 0.5f, Quaternion.Euler(270, 0, 90));
        }

        while (tiempoTranscurrido < tiempoTransición)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / tiempoTransición;
            float valorTiempo = Mathf.Lerp(tiempoInicial, tiempoFinal, t);

            // Actualiza la propiedad "_tiempo" de cada material en la lista de mallas
            foreach (Renderer renderer in mallas)
            {
                if (renderer != null)
                {
                    renderer.material.SetFloat("_tiempo", valorTiempo);
                }
            }

            yield return null;
        }

        // Asegura que el valor final sea exacto
        foreach (Renderer renderer in mallas)
        {
            if (renderer != null)
            {
                renderer.material.SetFloat("_tiempo", tiempoFinal);
            }
        }
    }

    [ContextMenu("Aplicar Efecto Stun")]
    public void AplicarEfectoStun()
    {
        StartCoroutine(EfectoStun());
    }

    private IEnumerator EfectoStun()
    {
        float tiempoTranscurrido = 0f;

        // Asegura que el efecto esté desactivado al iniciar
        SetEfectoGolpe(false);

        while (tiempoTranscurrido < tiempoStun)
        {
            // Alterna el valor de "_efectoGolpe" entre true y false
            bool activar = (tiempoTranscurrido % (frecuenciaStun * 2)) < frecuenciaStun;
            SetEfectoGolpe(activar);

            tiempoTranscurrido += frecuenciaStun;
            yield return new WaitForSeconds(frecuenciaStun);
        }

        // Asegura que el efecto esté desactivado al finalizar
        SetEfectoGolpe(false);
    }

    private void SetEfectoGolpe(bool valor)
    {
        foreach (Renderer renderer in mallas)
        {
            if (renderer != null)
            {
                renderer.material.SetFloat("_efectoGolpe", valor ? 1.0f : 0.0f);
            }
        }
    }
}
