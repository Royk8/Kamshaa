using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EventsManager : MonoBehaviour
{
    [field: Header("Player")]
    [field: SerializeField] public EventReference PlayerStep { get; private set; }
    [field: SerializeField] public EventReference PlayerGround { get; private set; }
    [field: SerializeField] public EventReference PlayerDash { get; private set; }
    [field: SerializeField] public EventReference PlayerAttack { get; private set; }
    [field: SerializeField] public EventReference PlayerDie { get; private set; }
    [field: SerializeField] public EventReference PlayerJump { get; private set; }

    

    [field: Header("Enemy")]

    [field: SerializeField] public EventReference EnemyAttack { get; private set; }

    [field: SerializeField] public EventReference EnemyDie { get; private set; }

    public static EventsManager Instance { get; private set; }


    [field: Header("Snapshots")]

    [field: SerializeField] public EventReference StunSnapshot { get; private set; }
    private void Awake()
    {
        if (Instance != null){
            Debug.LogError("Hay mas de un EventManger");
            
        }
        Instance = this;
    }

}
