using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace YarnSpinnerUnityExtensions
{
    public struct StoryData
    {
        public string characterName;
        public int characterID;
        public string dialogueText;
    }    
    public class StoryHandler : MonoBehaviour
    {
        [Header("DATA")]
        [SerializeField] private CharacterUIDataScriptableObject characterUIDataScriptableObject;
        [SerializeField] private UIOptionsDataScriptableObject UIOptionsDataScriptableObject;

        [Header("STORY SCENE OBJECTS")]
        [SerializeField] private Transform storyTextParent;
        [SerializeField] private DialogueTextBox tempTextBox ;
        [SerializeField] private DialogueTextBox dialogueTextBoxPrefab;
        [SerializeField] public OptionButtonTextBox optionButtonPrefab;

        private UIOptionsData _UIOptionsData;
        
        private float _optionButtonFadeDuration;
        private float _textBoxFadeInDuration;
        private TextBoxUIValues _textBoxUIValues = new TextBoxUIValues();

        private RectTransform _rectTransfrom;
        private ContentSizeFitter _contentSizeFitter;
        
        private List<OptionButtonTextBox> optionButtonTextBoxes = new List<OptionButtonTextBox>();

        private void Awake()
        {
            // Assign UI Data Options for this Story
            _UIOptionsData = UIOptionsDataScriptableObject.UIOptionsData;
            _textBoxUIValues = UIOptionsDataScriptableObject.textBoxUIValues;
            
            _optionButtonFadeDuration = _UIOptionsData.optionButtonFadeDuration;
            _textBoxFadeInDuration = _UIOptionsData.textBoxFadeInDuration;

            _rectTransfrom = tempTextBox.GetComponent<RectTransform>();
        }

        private StoryData GetStoryDataFromStoryText(string storyText)
        {
            var characterNameEndIndex = storyText.IndexOf(":", StringComparison.Ordinal);

            var ID = storyText.Substring(0, 1);
            
            var characterName = storyText.Substring(1,  characterNameEndIndex - 1);

            var dialogueText = storyText.Substring(characterNameEndIndex + 2);

            var storyData = new StoryData {dialogueText = dialogueText, characterID = int.Parse(ID), characterName = characterName};

            return storyData;
        }

        private float GetHeightOfTempTextBox(CharacterUIData characterUIData, TextBoxUIValues textBoxUIValues, string storyText)
        {
            tempTextBox.AddTextToDialogue(storyText);
            
            tempTextBox.SetDialogueAlignment(characterUIData.textBoxAlignment, textBoxUIValues);

            var verticalPadding = textBoxUIValues.verticalPadding + textBoxUIValues.interTextBoxSpacingValue;
            
            tempTextBox.SetTextBoxVerticalPadding(verticalPadding);
            
            Canvas.ForceUpdateCanvases();
            
            return _rectTransfrom.rect.height;
        }
        
        public void CreateStoryDialogueForThisText(string storyText)
        {
            var storyData = GetStoryDataFromStoryText(storyText);
            
            var characterUIData =
                characterUIDataScriptableObject.characterUIDatas[storyData.characterID - 1];

            var height = GetHeightOfTempTextBox(characterUIData, _textBoxUIValues, storyData.dialogueText);

            DialogueTextBox dialogueTextBox;
            
            if (optionButtonTextBoxes.Count > 0)
            {
                dialogueTextBox = optionButtonTextBoxes[0];

                optionButtonTextBoxes.RemoveAt(0);
            }
            else
            {
                dialogueTextBox = Instantiate(dialogueTextBoxPrefab, storyTextParent);
            }
            
            InstantiateTextBoxObject(dialogueTextBox, characterUIData, height, storyData.dialogueText, _textBoxFadeInDuration);
        }
        
        private void InstantiateTextBoxObject(DialogueTextBox prefab, CharacterUIData characterUIData, float height, string storyText, float animationDuration)
        {
            prefab.SetDialogueUI(characterUIData, _textBoxUIValues, height, animationDuration);

            StartCoroutine(SetTextBoxText(prefab, storyText, animationDuration));
        }

        private IEnumerator SetTextBoxText(DialogueTextBox textBox, string storyText, float animationDuration)
        {
            yield return new WaitForSeconds(animationDuration);

            textBox.InstantiateDialogue(storyText, CharacterUIData.TypeOfTextBox.Dialogue, animationDuration);
        }

        public OptionButtonTextBox GenerateOptionForThisNode(string optionText)
        {
            var storyData = GetStoryDataFromStoryText(optionText);

            var characterUIData =
                characterUIDataScriptableObject.characterUIDatas[storyData.characterID - 1];
            
            var height = GetHeightOfTempTextBox(characterUIData, _textBoxUIValues, storyData.dialogueText);
            
            var optionDialogueGameObject = Instantiate(optionButtonPrefab, storyTextParent);
            
            InstantiateTextBoxObject(optionDialogueGameObject, characterUIData, height, storyData.dialogueText, _textBoxFadeInDuration);
            
            optionButtonTextBoxes.Add(optionDialogueGameObject);
            
            return optionDialogueGameObject;
        }

        public void DestroyRemainingOptionDialoguesOnSelection(int chosenOptionID)
        {
            var optionDialogue = optionButtonTextBoxes[chosenOptionID];
            
            for (var i = 0; i < optionButtonTextBoxes.Count; i++)
            {
                optionButtonTextBoxes[i].ToggleOptionVisibility(false);
                optionButtonTextBoxes[i].ClearText();
                optionButtonTextBoxes[i].dialogueButton.interactable = false;
                
                if (i == 0)
                {
                    var height = GetHeightOfTempTextBox(optionDialogue.ThisCharacterUIData, _textBoxUIValues, optionDialogue.Dialogue);

                    InstantiateTextBoxObject(optionButtonTextBoxes[i], optionDialogue.ThisCharacterUIData, height, optionDialogue.Dialogue, _optionButtonFadeDuration);
                }
            }
            
            // Remove the first Option Dialogue as it's already been used for the selection
            optionButtonTextBoxes.RemoveAt(0);
        }
    }
}
