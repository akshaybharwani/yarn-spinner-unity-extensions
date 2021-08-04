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
        [Header("STORY UI DATA")]
        [SerializeField] private StoryUIDataScriptableObject _storyUIDataScriptableObject;
        
        [Header("STORY SCENE OBJECTS")]
        [SerializeField] private Transform _storyTextParent;
        [SerializeField] private Transform _storyTempTextParent;
        [SerializeField] private GameObject _dummyPanel;
        [SerializeField] private StoryDialogue _storyDialoguePrefab;
        [SerializeField] public OptionDialogue _optionDialoguePrefab;

        [Header("STORY UI OPTIONS")]
        [SerializeField] private float _optionDialoguePrefabFadeDuration;
        [SerializeField] private float _dialogueFadeInDuration = 1f;
        [SerializeField] private int _interDialogueSpacingValue;
        [SerializeField] private int _dialoguePaddingValue; 
        
        private List<OptionDialogue> _optionDialogues = new List<OptionDialogue>();

        private void Start()
        {
            _storyTextParent.GetComponent<VerticalLayoutGroup>().spacing = _interDialogueSpacingValue;
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

        private IEnumerator InstantiateDialogueGameObject(Component storyDialogueTempGameObject)
        {
            yield return new WaitForSeconds(0.01f);
            
            var height = GetHeightOfThisRect(storyDialogueTempGameObject);

            var dummyPanel = Instantiate(_dummyPanel, _storyTextParent);
            dummyPanel.GetComponent<LayoutElement>().DOMinSize(new Vector2(0, height), _dialogueFadeInDuration);

            StartCoroutine(FadeInDialogue(storyDialogueTempGameObject, dummyPanel));
        }

        private IEnumerator FadeInDialogue(Component storyDialogueTempGameObject, GameObject dummyPanel)
        {
            yield return new WaitForSeconds(_dialogueFadeInDuration);
            
            storyDialogueTempGameObject.transform.SetParent(_storyTextParent);
            storyDialogueTempGameObject.GetComponent<CanvasGroup>().DOFade(1, _dialogueFadeInDuration);

            dummyPanel.GetComponent<LayoutElement>().ignoreLayout = true;
        }
        
        public void CreateStoryDialogueForThisText(string storyText)
        {
            if (storyText.Contains("-"))
            {
                var storyData = GetStoryDataFromStoryText(storyText);

                var storyDialogueGameObject = Instantiate(_storyDialoguePrefab, _storyTempTextParent);

                var characterUIData =
                    _storyUIDataScriptableObject.storyUIData.characterUIDatas[storyData.characterID - 1];
                
                storyDialogueGameObject.InstantiateDialogue(storyData.dialogueText, characterUIData, characterUIData.dialogueBackgroundColor, _dialoguePaddingValue);

                StartCoroutine(InstantiateDialogueGameObject(storyDialogueGameObject));
            }
        }

        public OptionDialogue GenerateOptionForThisNode(string optionText)
        {
            var storyData = GetStoryDataFromStoryText(optionText);

            var optionDialogueGameObject = Instantiate(_optionDialoguePrefab, _storyTempTextParent);

            var characterUIData =
                _storyUIDataScriptableObject.storyUIData.characterUIDatas[storyData.characterID - 1];
            
            optionDialogueGameObject.InstantiateDialogue(storyData.dialogueText, characterUIData, characterUIData.optionBackgroundColor, _dialoguePaddingValue);
            
            StartCoroutine(InstantiateDialogueGameObject(optionDialogueGameObject));
            
            _optionDialogues.Add(optionDialogueGameObject);
            
            return optionDialogueGameObject;
        }

        public void DestroyRemainingOptionDialoguesOnSelection
            (int chosenOptionID)
        {
            var optionDialogue = _optionDialogues[chosenOptionID];
            
            for (var i = 0; i < _optionDialogues.Count; i++)
            {
                if (i == 0)
                {
                    _optionDialogues[i].ChangeOptionToDialogue(optionDialogue.Dialogue, optionDialogue.ThisCharacterUIData.dialogueBackgroundColor);
                    optionDialogue.dialogueButton.interactable = false;
                }
                else
                {
                    StartCoroutine(DestroyGameObject(0, _optionDialogues[i].gameObject));
                }
            }

            _optionDialogues.Clear();
        }
        
        private IEnumerator DestroyGameObject(float duration, GameObject thisGameObject) 
        {
            yield return new WaitForSeconds(duration);
            
            Destroy(thisGameObject);
        }

        private static float GetHeightOfThisRect(Component component)
        {
            var rect = component.GetComponent<RectTransform>().rect;
            return rect.height;
        }
    }
}
