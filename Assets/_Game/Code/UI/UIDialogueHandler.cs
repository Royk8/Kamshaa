using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
    public InputAdapter inputAdapter;
    private bool isDisplayingText;
    public UnityEvent OnDialogueEnd;
    public FmodAudioTable fmodAudioTable;

    private void Start()
    {
        textLocalizationFinder = FindObjectOfType<TextLocalizationFinder>();
        fmodAudioTable = GetComponent<FmodAudioTable>();
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

    public void TogglePanel(bool toggle)
    {
        dialoguePanel.SetActive(toggle);
        inputAdapter.ToggleToUI(toggle);
    }

    public IEnumerator ShowDialogue(List<DialogueLine> lines)
    {
        TogglePanel(true);

        bool isNextMessage = false;

        void HandleOnNextMessage(InputAction.CallbackContext context)
        {
            //Debug.Log("Next message event");
            isNextMessage = true;
        }

        inputAdapter.OnNextMessage += HandleOnNextMessage;

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
                //Debug.Log("Speaker: " + speaker);
                leftImage.gameObject.SetActive(false);
                rightImage.gameObject.SetActive(true);
                rightImage.sprite = dialogueSpriteMapScriptable.GetSprite(speaker);
            }

            string key = line.key;
            //Play fmod audio
            fmodAudioTable.PlayDialogue(key);

            if (line.key.StartsWith("C2"))
            {
                Movement mov = inputAdapter.gameObject.GetComponent<Movement>();
                if(mov != null)
                {
                    mov.extraJumps = 1;
                }
            }

            StartCoroutine(DisplayDialogueAnimation(line.text));
            //Debug.Log("Waiting for next message");
            yield return new WaitUntil(() => isNextMessage);
            if(isDisplayingText)
            {
                isNextMessage = false;
                conversationText.text = line.text;
                isDisplayingText = false;
                
                yield return new WaitUntil(() => isNextMessage);
                fmodAudioTable.StopDialogue(key);
            }
            //Debug.Log("Done waiting for next message");

            isNextMessage = false;
        }
        inputAdapter.OnNextMessage -= HandleOnNextMessage;

        TogglePanel(false);
        
        OnDialogueEnd?.Invoke();

        yield return null;
    }

    private IEnumerator DisplayDialogueAnimation(string line)
    {
        string currentText = "";
        line += "E";
        WaitForSeconds wait = new WaitForSeconds(1f / lettersPerSecond);
        isDisplayingText = true;
        for(int i = 0; i < line.Length; i++)
        {
            if(!isDisplayingText)
            {
                break;
            }
            currentText = line.Substring(0, i) + "<color=#00000000>" + line.Substring(i) + "</color>";
            conversationText.text = currentText;
            yield return wait;
        }
        isDisplayingText = false;
    }


}
