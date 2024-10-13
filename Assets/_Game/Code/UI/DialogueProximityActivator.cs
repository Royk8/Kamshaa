using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueProximityActivator : MonoBehaviour
{
    public string dialogueID;
    public UIDialogueHandler dialogueHandler;
    public bool isUsed = false;
    public float delay = 1f;

    public IEnumerator StartDialogue(string dialogueID)
    {
        yield return new WaitForSeconds(delay);
        dialogueHandler.InitDialogue(dialogueID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !isUsed)
        {
            isUsed = true;
            StartCoroutine(StartDialogue(dialogueID));
        }
    }
}
