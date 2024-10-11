using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueHandler : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text conversationText;
    public Image leftImage;
    public Image rightImage;
    public string conversationId;
    TextLocalizationFinder textLocalizationFinder;

    private void Start()
    {
        textLocalizationFinder = FindObjectOfType<TextLocalizationFinder>();
    }

    public void InitDialogue(string conversationId)
    {
        List<DialogueLine> lines = textLocalizationFinder.GetTexts(conversationId);
        StartCoroutine(ShowDialogue(lines));
    }

    [ContextMenu("Show")]
    public void Show()
    {
        InitDialogue(conversationId);
    }

    public IEnumerator ShowDialogue(List<DialogueLine> lines)
    {
        dialoguePanel.SetActive(true);
        WaitForSeconds wait = new WaitForSeconds(2f);

        foreach (var line in lines)
        {
            conversationText.text = line.text;
            if(line.speaker == "Kaamsha")
            {
                leftImage.gameObject.SetActive(true);
                rightImage.gameObject.SetActive(false);
            }
            else
            {
                //rightImage.sprite = line.sprite;
                leftImage.gameObject.SetActive(false);
                rightImage.gameObject.SetActive(true);
            }
            yield return wait;
        }

        yield return null;
    }


}
