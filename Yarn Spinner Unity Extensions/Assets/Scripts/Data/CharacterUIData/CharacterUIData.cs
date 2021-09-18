using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterUIData
{
    public enum TextBoxAlignment {
        left,
        center,
        right
    }

    public enum TypeOfTextBox
    {
        Dialogue,
        Option
    }

    public string characterID;
    public Color32 dialogueBackgroundColor;
    public Sprite dialogueBackgroundImage;
    public Color32 optionBackgroundColor;
    public Sprite optionBackgroundImage;
    public TMP_FontAsset fontAsset;
    public Color32 fontColor;
    public TextBoxAlignment textBoxAlignment;
}

