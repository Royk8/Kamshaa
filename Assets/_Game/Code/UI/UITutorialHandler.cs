using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITutorialHandler : MonoBehaviour
{
    public float displayTime = 5;
    public GameObject dialoguePanel;
    public Text conversationText;
    public Image image;
    public float lettersPerSecond = 10f;
    public string conversationId;
    public TextLocalizationFinder textLocalizationFinder;
    public InputAdapter inputAdapter;
    private bool isDisplayingText;
    public Sprite xboxEast;
    public Sprite xboxWest;
    public Sprite xboxsouth;
    public Sprite Space;
    public Sprite left;
    public Sprite right;

    private void Start()
    {
        textLocalizationFinder = FindObjectOfType<TextLocalizationFinder>();
    }

    public void InitDialogue(string conversationId)
    {
        Debug.Log("Buscando " +  conversationId);
        string line = textLocalizationFinder.GetText(conversationId);
        Debug.Log("Encontrada: " + line);

        StartCoroutine(ShowDialogue(line));
    }

    private void SetTutorialText(string text)
    {
        conversationText.text = text;
        Debug.Log("Text to display: " + text);
    }

    IEnumerator ShowDialogue(string line)
    {
        yield return new WaitForSeconds(1f);
        dialoguePanel.SetActive(true);
        bool controller = inputAdapter.IsController();
        Sprite sprite = null;


        Debug.Log(line);
        //Get the seccion in the text between <>
        string[] split = line.Split('<', '>');

        Debug.Log(split.Length);
        Debug.Log(split[0]);


        if (split.Length < 2)
        {
            string text = split[0];
            SetTutorialText(text);
            image.gameObject.SetActive(false);
            yield return new WaitForSeconds(displayTime);


            image.gameObject.SetActive(true);
            dialoguePanel.SetActive(false);
            yield break;
        }

        if (split[1].Contains("East"))
        {
            if (controller)
            {
                sprite = xboxEast;
            }
            else
            {
                sprite = left;
            }
        }
        else if (split[1].Contains("West"))
        {
            if (controller)
            {
                sprite = xboxWest;
            }
            else
            {
                sprite = right;
            }
        }
        else if (split[1].Contains("South"))
        {
            if (controller)
            {
                sprite = xboxsouth;
            }
            else
            {
                sprite = Space;
            }
        }

        SetTutorialText(split[0]);
        image.sprite = sprite;
        yield return new WaitForSeconds(displayTime);
        dialoguePanel.SetActive(false);
    }

    [ContextMenu("Show")]
    public void Show()
    {
        InitDialogue(conversationId);
    }
}
