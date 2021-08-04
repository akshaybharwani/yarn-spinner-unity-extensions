using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterUIData
{
    public enum DialogueAlignment {
        left,
        center,
        right
    }
    
    public string characterID;
    public Color32 dialogueBackgroundColor;
    public Color32 optionBackgroundColor;
    public Image backgroundImage;
    public DialogueAlignment dialogueAlignment;
}

