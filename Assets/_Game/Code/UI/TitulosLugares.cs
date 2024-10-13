using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitulosLugares : MonoBehaviour
{
    public Sprite[] sprAzul;
    public Sprite[] sprRojo;
    public Sprite[] sprVerde;
    public TiposLugares ultimoTipo;

    public Image imTitulo;
    public float velocidadFPS = 5;

    private Sprite[] sprActivo;
    private Coroutine animacionActiva;

    [ContextMenu("Activar Rojo")]
    public void ActivarRojo()
    {
		if (ultimoTipo != TiposLugares.rojo)
		{
            sprActivo = sprRojo;
            IniciarAnimacion();
            ultimoTipo = TiposLugares.rojo;
        }
    }

    [ContextMenu("Activar Verde")]
    public void ActivarVerde()
    {
        if (ultimoTipo != TiposLugares.rojo)
        {
            sprActivo = sprVerde;
            IniciarAnimacion();
            ultimoTipo = TiposLugares.verde;
        }
    }

    [ContextMenu("Activar Azul")]
    public void ActivarAzul()
    {
        if (ultimoTipo != TiposLugares.azul)
        {
            sprActivo = sprAzul;
            IniciarAnimacion();
            ultimoTipo = TiposLugares.azul;
        }
    }

    private void IniciarAnimacion()
    {
        // Detiene cualquier animación previa
        if (animacionActiva != null)
        {
            StopCoroutine(animacionActiva);
        }

        // Inicia la animación con el sprite activo
        animacionActiva = StartCoroutine(Animar());
    }

    private IEnumerator Animar()
    {
        int index = 0;
        float intervalo = 1f / velocidadFPS;

        while (index < sprActivo.Length)
        {
            if (sprActivo == null || sprActivo.Length == 0)
                yield break;

            imTitulo.sprite = sprActivo[index];
            index = (index + 1);

            yield return new WaitForSeconds(intervalo);
        }
    }
}

public enum TiposLugares
{
    neutral,
    verde, 
    rojo,
    azul
}