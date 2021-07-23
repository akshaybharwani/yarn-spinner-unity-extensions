using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] protected Image _dialogueBackgroundImage;
    private RectTransform _backgroundRectTransform;
    protected CharacterUIData _thisCharacterUIData;
    
    private VerticalLayoutGroup _verticalLayoutGroup;

    private void Awake()
    {
        _verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        _backgroundRectTransform = _dialogueBackgroundImage.GetComponent<RectTransform>();
    }

    public void InstantiateDialogue(string dialogueText, CharacterUIData characterUIData, Color32 color32)
    {
        _thisCharacterUIData = characterUIData;
        
        _dialogueText.text = dialogueText;

        switch (characterUIData.dialogueAlignment)
        {
            case CharacterUIData.DialogueAlignment.left:
                _verticalLayoutGroup.padding.right = 170;
                _backgroundRectTransform.offsetMax = new Vector2(-150, _backgroundRectTransform.offsetMax.y);
                break;
            case CharacterUIData.DialogueAlignment.right:
                _verticalLayoutGroup.padding.left = 170;
                _backgroundRectTransform.offsetMin = new Vector2(150, _backgroundRectTransform.offsetMin.y);
                break;
            case CharacterUIData.DialogueAlignment.center:
                _verticalLayoutGroup.padding.left = 100;
                _backgroundRectTransform.offsetMin = new Vector2(80, _backgroundRectTransform.offsetMin.y);
                _verticalLayoutGroup.padding.right = 100;
                _backgroundRectTransform.offsetMax = new Vector2(-80, _backgroundRectTransform.offsetMax.y);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _dialogueBackgroundImage.color = color32;
    }
}
