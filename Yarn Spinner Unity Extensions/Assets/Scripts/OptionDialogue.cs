using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OptionDialogue : StoryDialogue
{
    public Button dialogueButton;

    public void ChangeOptionToDialogue(string dialogueText, Color32 color32)
    {
        _dialogueText.text = dialogueText;
        _dialogueBackgroundImage.DOColor(color32, 1.5f);
    }
}
