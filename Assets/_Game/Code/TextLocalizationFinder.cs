using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Tables;


public class TextLocalizationFinder : MonoBehaviour
{
    public LocalizedStringTable localizedStringTable;
    public string entryId;
    private StringTable dialogsTable;

    private Dictionary<string, object> _passedData = new Dictionary<string, object> { ["Count"] = 3 };


    private void Awake()
    {
        StartCoroutine(LoadDialogTable());
    }

    [ContextMenu("Say")]
    public void Say()
    {
        //Debug.Log(GetText("D1Intro"));
        GetTexts(entryId);
    }

    IEnumerator LoadDialogTable()
    {
        StringTable tableLoading = localizedStringTable.GetTable();
        yield return tableLoading;
        dialogsTable = tableLoading;
    }

    public string GetText(string entry)
    {
        foreach (var item in dialogsTable.Values)
        {
            if (item.Key.StartsWith(entry))
            {
                return item.GetLocalizedString();
            }
        }
        return null;
    }


    public void GetTexts()
    {
        foreach (var item in dialogsTable.Values)
        {
            Debug.Log(item.LocalizedValue);
            IMetadataCollection met = item.SharedEntry.Metadata;
            Comment comment = met.GetMetadata<Comment>();
            Debug.Log(comment.CommentText);
        }
    }

    public List<DialogueLine> GetTexts(string conversationId)
    {
        List<DialogueLine> texts = new List<DialogueLine>();
        foreach (var item in dialogsTable.Values)
        {
            if(item.Key.StartsWith(conversationId))
            {
                texts.Add(new DialogueLine
                {
                    key = item.Key,
                    text = item.LocalizedValue,
                    speaker = item.SharedEntry.Metadata.GetMetadata<Comment>().CommentText
                });
            }
        }

        //foreach (var item in texts)
        //{
        //    Debug.Log(item.speaker + ": " + item.text);
        //}

        return texts;
    }
}

public struct DialogueLine
{
    public string key;
    public string text;
    public string speaker;
}
