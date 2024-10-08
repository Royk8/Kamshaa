using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EventsManager : MonoBehaviour
{
    [field: Header("SFX")]
    [field: SerializeField] public EventReference BunnyHurt { get; private set; }
    [field: SerializeField] public EventReference BearHurt { get; private set; }
    [field: SerializeField] public EventReference HellephantHurt { get; private set; }
    [field: SerializeField] public EventReference PlayerHurt { get; private set; }
    [field: SerializeField] public EventReference PlayerShoot { get; private set; }

    public static EventsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null){
            Debug.LogError("Hay mas de un EventManger");
            
        }
        Instance = this;
    }

}
