using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFmod : MonoBehaviour
{

    public void PlayEvent(string eventoPath)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventoPath, GetComponent<Transform>().position);

        //AudioManager.Instance.PlayOneShot(EventsManager.Instance.eventname, this.transform.position);
    }

}
