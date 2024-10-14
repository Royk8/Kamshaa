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
        string line = textLocalizationFinder.GetText(conversationId);
        StartCoroutine(ShowDialogue(line));
    }

    IEnumerator ShowDialogue(string line)
    {
        yield return new WaitForSeconds(1f);
        dialoguePanel.SetActive(true);
        bool controller = inputAdapter.IsController();
        Sprite sprite = null;


        //Get the seccion in the text between <>
        string[] split = line.Split('<', '>');

        if (split.Length < 2)
        {
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

        image.sprite = sprite;
        string text = split[0];
        Debug.Log(split[1]);
        conversationText.text = text;
        yield return new WaitForSeconds(displayTime);
        dialoguePanel.SetActive(false);
    }

    [ContextMenu("Show")]
    public void Show()
    {
        InitDialogue(conversationId);
    }
}
