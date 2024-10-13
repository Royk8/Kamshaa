using UnityEngine;
using UnityEditor;

public class TeleportarPlayer : MonoBehaviour
{
    private static Transform player;

    // Inicializa el menú en el editor
    [MenuItem("Morion/Teleportar/Neutral")]
    private static void TeleportarNeutral()
    {
        TeleportarPlayerTo(new Vector3(0, 4, 11));
    }

    [MenuItem("Morion/Teleportar/Agua")]
    private static void TeleportarAgua()
    {
        TeleportarPlayerTo(new Vector3(-83, 4, 4));
    }

    [MenuItem("Morion/Teleportar/Fuego")]
    private static void TeleportarFuego()
    {
        TeleportarPlayerTo(new Vector3(8, 4, -108));
    }

    [MenuItem("Morion/Teleportar/Tierra")]
    private static void TeleportarTierra()
    {
        TeleportarPlayerTo(new Vector3(120, 4, 3));
    }

    private static void TeleportarPlayerTo(Vector3 posicion)
    {
        // Encuentra el objeto llamado "Player" en la escena
        if (player == null)
        {
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("No se encontró un objeto llamado 'Player' en la escena.");
                return;
            }
        }

        // Cambia la posición del jugador
        player.position = posicion;
        Debug.Log($"Player teletransportado a la posición {posicion}");
    }
}
