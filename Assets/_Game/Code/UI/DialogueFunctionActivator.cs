using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueFunctionActivator : MonoBehaviour
{
    public string dialogueID;
    public UIDialogueHandler dialogueHandler;
    public bool isUsed = false;
    public float delay = 1f;

    public IEnumerator StartDialogue(string dialogueID)
    {
        yield return new WaitForSeconds(delay);
        if (dialogueHandler == null)
        {
            dialogueHandler = FindObjectOfType<UIDialogueHandler>();
            if (dialogueHandler == null)
            {
                yield break;
            }

            dialogueHandler.InitDialogue(dialogueID);
        }
    }

    public void ActivateDialogue()
    {
        if (!isUsed)
        {
            isUsed = true;
            StartCoroutine(StartDialogue(dialogueID));
        }
    }
}
