using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class ParticleFmod : MonoBehaviour
{

    

    [field: SerializeField] public EventReference Event { get; private set; }

    public ParticleSystem ps;

    public GameObject parent;

    private FMOD.Studio.EventInstance evento;
    void Start()
    {
        if (ps == null) {
            ps = GetComponentInParent<ParticleSystem>();
        }
        evento = AudioManager.Instance.NuevaInstancia(Event);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(evento, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }
    // Update is called once per frame
    void OnGUI()
    {
        if (ps.isEmitting || parent.activeSelf)
        {
            evento.start();
        }
        
        if (ps.isEmitting == false || parent.activeSelf == false) 
        {

            evento.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }


    }
}
