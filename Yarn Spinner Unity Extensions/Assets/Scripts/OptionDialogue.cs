using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OptionDialogue : StoryDialogue
{
    public Button dialogueButton;
    
    public void ChangeOptionToDialogue()
    {
        _dialogueBackgroundImage.DOColor(_thisCharacterUIData.dialogueBackgroundColor, 1f);
    }
}
