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
    [field: SerializeField] public EventReference PlayerJump { get; private set; }

    

    [field: Header("Guardianes")]

    [field: SerializeField] public EventReference Teshemba1Attack { get; private set; }

    [field: SerializeField] public EventReference Teshemba1Hurt { get; private set; }

    [field: SerializeField] public EventReference Teshemba2Attack { get; private set; }

    [field: SerializeField] public EventReference Teshemba2Hurt { get; private set; }

    [field: SerializeField] public EventReference Yahtu1Attack { get; private set; }

    [field: SerializeField] public EventReference Yahtu1Walking { get; private set; }

    [field: SerializeField] public EventReference Yahtu1Hurt { get; private set; }

    [field: SerializeField] public EventReference Yahtu2Attack { get; private set; }

    [field: SerializeField] public EventReference Yahtu2Hurt { get; private set; }

    [field: SerializeField] public EventReference Yahuar1Attack { get; private set; }

    [field: SerializeField] public EventReference Yahuar1Hurt { get; private set; }

    [field: SerializeField] public EventReference Yahuar2Attack { get; private set; }

    [field: SerializeField] public EventReference Yahuar2Hurt { get; private set; }

    [field: Header("Bosses")]
    [field: SerializeField] public EventReference TeshembaAttack { get; private set; }

    [field: SerializeField] public EventReference TeshembaHurt { get; private set; }

    [field: SerializeField] public EventReference YahtuAttack { get; private set; }

    [field: SerializeField] public EventReference YahtuHurt { get; private set; }

    [field: SerializeField] public EventReference YahuarAttack { get; private set; }

    [field: SerializeField] public EventReference YahuarHurt { get; private set; }



    [field: Header("Snapshots")]

    [field: SerializeField] public EventReference StunSnapshot { get; private set; }


    public static EventsManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null){
            Debug.LogError("Hay mas de un EventManger");
            
        }
        Instance = this;
    }

}
