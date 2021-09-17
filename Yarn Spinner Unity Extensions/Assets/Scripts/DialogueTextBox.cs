using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTextBox : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _dialogueText;
    [SerializeField] protected Image _textBoxBackgroundImage;
    private string _dialogue;
    private RectTransform _backgroundRectTransform;
    private LayoutElement _layoutElement;
    private CanvasGroup _canvasGroup;

    public CharacterUIData ThisCharacterUIData { get; private set; }

    public float Height { get; private set; }

    public string Dialogue
    {
        get => _dialogue;
    }

    private VerticalLayoutGroup _verticalLayoutGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        _layoutElement = GetComponent<LayoutElement>();
        _backgroundRectTransform = _textBoxBackgroundImage.GetComponent<RectTransform>();
    }

    public void ClearText()
    {
        _dialogueText.text = "";
    }

    public void AddTextToDialogue(string storyText)
    {
        _dialogueText.text = storyText;
    }

    public void SetDialogueUI(CharacterUIData characterUIData, TextBoxUIValues textBoxUIValues, float height, float animationDuration)
    {
        ThisCharacterUIData = characterUIData;

        SetDialogueAlignment(characterUIData.textBoxAlignment, textBoxUIValues);

        Height = height;

        _layoutElement.DOMinSize(new Vector2(0, height), animationDuration);

        var verticalPadding = textBoxUIValues.verticalPadding + textBoxUIValues.interTextBoxSpacingValue;

        StartCoroutine(SetTextBoxVerticalPaddingAfterHeightIsSet(verticalPadding, animationDuration));
    }

    public void SetDialogueAlignment(CharacterUIData.TextBoxAlignment textBoxAlignment, TextBoxUIValues textBoxUIValues)
    {
        var verticalMargin = textBoxUIValues.verticalMargin + textBoxUIValues.interTextBoxSpacingValue;

        switch (textBoxAlignment)
        {
            case CharacterUIData.TextBoxAlignment.left:
                SetBackgroundSize(textBoxUIValues.minHorizontalMargin,
                    textBoxUIValues.maxHorizontalMargin,
                    verticalMargin,
                    verticalMargin);

                SetTextBoxHorizontalPadding(textBoxUIValues.minHorizontalMargin + textBoxUIValues.horizontalPadding,
                    textBoxUIValues.maxHorizontalMargin + textBoxUIValues.horizontalPadding);

                break;
            case CharacterUIData.TextBoxAlignment.right:
                SetBackgroundSize(textBoxUIValues.maxHorizontalMargin,
                    textBoxUIValues.minHorizontalMargin,
                    verticalMargin,
                    verticalMargin);

                SetTextBoxHorizontalPadding(textBoxUIValues.maxHorizontalMargin + textBoxUIValues.horizontalPadding,
                    textBoxUIValues.minHorizontalMargin + textBoxUIValues.horizontalPadding);

                break;
            case CharacterUIData.TextBoxAlignment.center:
                SetBackgroundSize(textBoxUIValues.centerHorizontalMargin,
                    textBoxUIValues.centerHorizontalMargin,
                    verticalMargin,
                    verticalMargin);

                var horizontalPadding = textBoxUIValues.centerHorizontalMargin + textBoxUIValues.horizontalPadding;

                SetTextBoxHorizontalPadding(horizontalPadding, horizontalPadding);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetTextBoxHorizontalPadding(int left, int right)
    {
        _verticalLayoutGroup.padding.left = left;
        _verticalLayoutGroup.padding.right = right;
    }

    public void SetTextBoxVerticalPadding(int value)
    {
        _verticalLayoutGroup.padding.top = value;
        _verticalLayoutGroup.padding.bottom = value;
    }

    private void SetBackgroundSize(int left, int right, int top, int bottom)
    {
        _backgroundRectTransform.offsetMin = new Vector2(left, bottom);
        _backgroundRectTransform.offsetMax = new Vector2(-right, -top);
    }

    private IEnumerator SetTextBoxVerticalPaddingAfterHeightIsSet(int topBottomPadding, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        SetTextBoxVerticalPadding(topBottomPadding);
    }

    public void InstantiateDialogue(string dialogueText, CharacterUIData.TypeOfTextBox typeOfTextBox, float animationDuration)
    {
        _dialogue = dialogueText;
        _dialogueText.text = dialogueText;

        _canvasGroup.DOFade(1, animationDuration);

        SetTextBoxBackground(ThisCharacterUIData, typeOfTextBox);
    }

    private void SetTextBoxBackground(CharacterUIData characterUIData, CharacterUIData.TypeOfTextBox typeOfTextBox)
    {
        switch (typeOfTextBox)
        {
            case CharacterUIData.TypeOfTextBox.Dialogue:
                if (characterUIData.dialogueBackgroundImage)
                {
                    _textBoxBackgroundImage.sprite = characterUIData.dialogueBackgroundImage;
                }
                else
                {
                    _textBoxBackgroundImage.color = characterUIData.dialogueBackgroundColor;
                }
                break;
            case CharacterUIData.TypeOfTextBox.Option:
                if (characterUIData.optionBackgroundImage)
                {
                    _textBoxBackgroundImage.sprite = characterUIData.optionBackgroundImage;
                }
                else
                {
                    _textBoxBackgroundImage.color = characterUIData.optionBackgroundColor;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(typeOfTextBox), typeOfTextBox, null);
        }
    }

    public void ToggleOptionVisibility(bool toggleValue)
    {
        _canvasGroup.alpha = toggleValue ? 1 : 0;
    }
}
