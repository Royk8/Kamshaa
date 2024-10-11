using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSpriteMap", menuName = "Scriptables/DialogueSpriteMap")]
public class DialogueSpriteMap : ScriptableObject
{
    public List<DialogueCharacter> characters;

    public Sprite GetSprite(string characterName)
    {
        foreach (var character in characters)
        {
            if (character.characterName == characterName)
            {
                Debug.Log("Sprite found for " + characterName);
                return character.sprite;
            }
        }

        return null;
    }
}

[Serializable]
public class DialogueCharacter
{
    public string characterName;
    public Sprite sprite;
}
