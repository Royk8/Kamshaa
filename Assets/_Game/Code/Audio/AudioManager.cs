using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static  AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null){
            Debug.LogError("Hay mas de un AudioManager");
            
        }
        Instance = this;
    }

    public void PlayOneShot(EventReference evento, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(evento, worldPos);
    }

    public EventInstance NuevaInstancia(EventReference evento)
    {
        EventInstance instanciaEvento = RuntimeManager.CreateInstance(evento);
        return instanciaEvento;
    }

}
