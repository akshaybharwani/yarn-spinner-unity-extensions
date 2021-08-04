using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryDialogue : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _dialogueText;
    [SerializeField] protected Image _dialogueBackgroundImage;
    private string dialogue;
    private RectTransform _backgroundRectTransform;
    protected CharacterUIData _thisCharacterUIData;

    public CharacterUIData ThisCharacterUIData => _thisCharacterUIData;

    public string Dialogue
    {
        get => dialogue;
    }
    
    private VerticalLayoutGroup _verticalLayoutGroup;

    private void Awake()
    {
        _verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        _backgroundRectTransform = _dialogueBackgroundImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        
    }

    public void InstantiateDialogue(string dialogueText, CharacterUIData characterUIData, Color32 color32, int topBottomPadding)
    {
        _thisCharacterUIData = characterUIData;

        dialogue = dialogueText;
        _dialogueText.text = dialogueText;

        _verticalLayoutGroup.padding.top = topBottomPadding;
        _verticalLayoutGroup.padding.bottom = topBottomPadding;
        
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
