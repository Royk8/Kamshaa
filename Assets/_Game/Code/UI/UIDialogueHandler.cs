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
    public float lettersPerSecond = 10f;
    public string conversationId;
    TextLocalizationFinder textLocalizationFinder;
    public DialogueSpriteMap dialogueSpriteMapScriptable;

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
            string speaker = line.speaker;
            if(speaker == "Kamshaá")
            {
                leftImage.gameObject.SetActive(true);
                rightImage.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Speaker: " + speaker);
                leftImage.gameObject.SetActive(false);
                rightImage.gameObject.SetActive(true);
                rightImage.sprite = dialogueSpriteMapScriptable.GetSprite(speaker);
            }

            yield return DisplayDialogueAnimation(line.text);
            yield return wait;
        }

        dialoguePanel.SetActive(false);

        yield return null;
    }

    private IEnumerator DisplayDialogueAnimation(string line)
    {
        string currentText = "";
        line += "E";
        WaitForSeconds wait = new WaitForSeconds(1f / lettersPerSecond);

        for(int i = 0; i < line.Length; i++)
        {
            currentText = line.Substring(0, i) + "<color=#00000000>" + line.Substring(i) + "</color>";
            conversationText.text = currentText;
            yield return wait;
        }
    }


}
