using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private StoryUIDataScriptableObject _storyUIDataScriptableObject;
        
        [SerializeField] private Transform _storyTextParent;
        [SerializeField] private StoryDialogue _storyDialoguePrefab;
        [SerializeField] public OptionDialogue _optionDialoguePrefab;

        [SerializeField] private float _optionDialoguePrefabFadeDuration;
        
        private List<OptionDialogue> _optionDialogues = new List<OptionDialogue>();

        public void CreateStoryDialogueForThisText(string storyText)
        {
            if (storyText.Contains("-"))
            {
                var storyData = GetStoryDataFromStoryText(storyText);
                
                var storyDialogueGameObject = Instantiate(_storyDialoguePrefab, _storyTextParent);

                var characterUIData =
                    _storyUIDataScriptableObject.storyUIData.characterUIDatas[storyData.characterID - 1];
                
                storyDialogueGameObject.InstantiateDialogue(storyData.dialogueText, characterUIData, characterUIData.dialogueBackgroundColor);
            }
        }

        private StoryData GetStoryDataFromStoryText(string storyText)
        {
            var characterNameEndIndex = storyText.IndexOf(":", StringComparison.Ordinal);

            var ID = storyText.Substring(1, 1);
            
            var characterName = storyText.Substring(2,  characterNameEndIndex - 1);

            var dialogueText = storyText.Substring(characterNameEndIndex + 2);

            var storyData = new StoryData {dialogueText = dialogueText,characterID = int.Parse(ID), characterName = characterName};

            return storyData;
        }

        public OptionDialogue GenerateOptionForThisNode(string optionText)
        {
            var storyData = GetStoryDataFromStoryText(optionText);

            var optionDialogueGameObject = Instantiate(_optionDialoguePrefab, _storyTextParent);
            
            var characterUIData =
                _storyUIDataScriptableObject.storyUIData.characterUIDatas[storyData.characterID - 1];
            
            optionDialogueGameObject.InstantiateDialogue(storyData.dialogueText, characterUIData, characterUIData.optionBackgroundColor);
            
            optionDialogueGameObject.dialogueButton.onClick.
                AddListener(() => 
                    DestroyRemainingOptionDialoguesOnSelection(optionDialogueGameObject));
            
            _optionDialogues.Add(optionDialogueGameObject);
            
            return optionDialogueGameObject;
        }

        private void DestroyRemainingOptionDialoguesOnSelection
            (OptionDialogue optionDialogue)
        {
            foreach (var dialogue in _optionDialogues)
            {
                if (optionDialogue != dialogue)
                {
                    var rect = dialogue.GetComponent<RectTransform>().rect;
                    var height = rect.height;
                    dialogue.GetComponent<VerticalLayoutGroup>().enabled = false;
                    var layoutElement = dialogue.GetComponent<LayoutElement>();
                    layoutElement.minHeight = height;
                    layoutElement.DOMinSize(Vector2.zero, _optionDialoguePrefabFadeDuration);
                    StartCoroutine(DestroyGameObject(_optionDialoguePrefabFadeDuration, dialogue.gameObject));
                }
            }

            optionDialogue.dialogueButton.interactable = false;
            optionDialogue.ChangeOptionToDialogue();

            _optionDialogues.Clear();
        }
        
        private IEnumerator DestroyGameObject(float duration, GameObject thisGameObject) 
        {
            yield return new WaitForSeconds(duration);
            
            Destroy(thisGameObject);
        }
    }
}
