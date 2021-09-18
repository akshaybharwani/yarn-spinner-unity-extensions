using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Yarn.Unity;

namespace YarnSpinnerUnityExtensions
{
    public struct StoryData
    {
        public string characterId;
        public string characterName;
        public string characterText;
        public bool hasCommand;
        public string commandName;
        public string commandText;
    }

    public enum TypeOfCommand
    {
        Image,
        BackgroundAudio,
        OneShotAudio,
        End
    }

    public class StoryHandler : MonoBehaviour
    {
        [Header("YARN SPINNER")] [SerializeField]
        private DialogueUI dialogueUI = null;

        [Header("DATA")]
        [SerializeField] private CharacterUIDataScriptableObject characterUIDataScriptableObject = null;
        [SerializeField] private UIOptionsDataScriptableObject UIOptionsDataScriptableObject = null;
        [SerializeField] private MediaDataScriptableObject mediaDataScriptableObject = null;

        [Header("STORY SCENE OBJECTS")]
        [SerializeField] private Transform storyTextParent = null;
        [SerializeField] private DialogueTextBox tempTextBox = null;
        [SerializeField] private DialogueTextBox dialogueTextBoxPrefab = null;
        [SerializeField] private OptionButtonTextBox optionButtonPrefab = null;
        [SerializeField] private StoryImageBox storyImageBox = null;

        [Header("AUDIO SOURCES")]
        [SerializeField] private AudioSource backgroundAudioSource = null;
        [SerializeField] private AudioSource oneShotAudioSource = null;

        private UIOptionsData _uiOptionsData;

        private float _optionButtonFadeDuration;
        private float _textBoxFadeInDuration;
        private TextBoxUIValues _textBoxUiValues = new TextBoxUIValues();

        private RectTransform _rectTransform;

        private List<OptionButtonTextBox> _optionButtonTextBoxes = new List<OptionButtonTextBox>();

        private void Awake()
        {
            // Assign UI Data Options for this Story
            _uiOptionsData = UIOptionsDataScriptableObject.UIOptionsData;
            _textBoxUiValues = UIOptionsDataScriptableObject.textBoxUIValues;

            _optionButtonFadeDuration = _uiOptionsData.optionButtonFadeDuration;
            _textBoxFadeInDuration = _uiOptionsData.textBoxFadeInDuration;

            _rectTransform = tempTextBox.GetComponent<RectTransform>();

            if (mediaDataScriptableObject.audioData.backgroundAudioClip)
                backgroundAudioSource.clip = mediaDataScriptableObject.audioData.backgroundAudioClip;
        }

        public void HandleNextNode(string storyText)
        {
            var assetData = GetStoryData(storyText);

            HandleStoryText(assetData);
        }

        private IEnumerator HandleCommand(StoryData storyData)
        {
            yield return new WaitForSeconds(_textBoxFadeInDuration);

            switch (storyData.commandName.Trim())
            {
                case nameof(TypeOfCommand.Image):
                    HandleImage(storyData);
                    break;
                case nameof(TypeOfCommand.BackgroundAudio):
                    HandleBackgroundAudioSource(storyData);
                    break;
                case nameof(TypeOfCommand.OneShotAudio):
                    HandleOneShotAudioSource(storyData);
                    break;
                case nameof(TypeOfCommand.End):
                    StartCoroutine(HandleEnd());
                    break;
            }
        }

        private IEnumerator HandleEnd()
        {
            yield return new WaitForSeconds(_textBoxFadeInDuration * 2);

            dialogueUI.DialogueComplete();
        }

        private void HandleImage(StoryData mediaData)
        {
            var sprite = Resources.Load<Sprite>(mediaDataScriptableObject.imagesResourcesPath + "/" + mediaData.commandText.Trim());

            if (sprite)
            {
                var height = sprite.texture.height;
                var imageBox = Instantiate(storyImageBox, storyTextParent);
                imageBox.InstantiateStoryImageBox(sprite, _textBoxFadeInDuration, height);
            }
        }

        private void HandleBackgroundAudioSource(StoryData mediaData)
        {
            if (mediaData.commandText == "On")
            {
                backgroundAudioSource.Play();
                backgroundAudioSource.DOFade(1, mediaDataScriptableObject.audioData.audioFadeDuration);
            }
            else
            {
                backgroundAudioSource.DOFade(0, mediaDataScriptableObject.audioData.audioFadeDuration);
                StartCoroutine(FadeOutAudioSource(backgroundAudioSource, mediaDataScriptableObject.audioData.audioFadeDuration));
            }
        }

        private IEnumerator FadeOutAudioSource(AudioSource audioSource, float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);

            audioSource.Stop();
        }

        private void HandleOneShotAudioSource(StoryData mediaData)
        {
            var audioClip = Resources.Load<AudioClip>(mediaDataScriptableObject.audioClipsResourcesPath + "/" + mediaData.commandText.Trim());

            if (audioClip)
            {
                oneShotAudioSource.PlayOneShot(audioClip);
                oneShotAudioSource.DOFade(1, mediaDataScriptableObject.audioData.audioFadeDuration);
            }
        }

        private void HandleStoryText(StoryData storyTextData)
        {
            if (storyTextData.hasCommand)
            {
                StartCoroutine(HandleCommand(storyTextData));
            }

            var characterUiData =
                characterUIDataScriptableObject.characterUIDatas[int.Parse(storyTextData.characterId) - 1];

            var height = GetHeightOfTempTextBox(characterUiData, _textBoxUiValues, storyTextData.characterText);

            DialogueTextBox dialogueTextBox;

            if (_optionButtonTextBoxes.Count > 0)
            {
                dialogueTextBox = _optionButtonTextBoxes[0];

                _optionButtonTextBoxes.RemoveAt(0);
            }
            else
            {
                dialogueTextBox = Instantiate(dialogueTextBoxPrefab, storyTextParent);
            }

            InstantiateTextBoxObject(dialogueTextBox, characterUiData, CharacterUIData.TypeOfTextBox.Dialogue, height, storyTextData.characterText, _textBoxFadeInDuration);
        }

        private StoryData GetStoryData(string storyText)
        {
            var id = storyText.Substring(0, 1);

            var characterNameIndex = storyText.IndexOf(":", StringComparison.Ordinal);
            var characterName = storyText.Substring(1,  characterNameIndex - 1);
            var characterTextIndex = characterNameIndex + 2;
            var characterText = "";

            var assetStartIndex = storyText.IndexOf("-", StringComparison.Ordinal);
            var assetNameIndex = storyText.IndexOf("~", StringComparison.Ordinal);

            var storyData = new StoryData();

            if (assetStartIndex != -1)
            {
                characterText = storyText.Substring(characterTextIndex, assetStartIndex - characterTextIndex);

                var assetName = "";
                var assetText = "";

                if (assetNameIndex != -1)
                {
                    assetName = storyText.Substring(assetStartIndex + 1, assetNameIndex - assetStartIndex - 2);
                    assetText = storyText.Substring(assetNameIndex + 2);
                }
                else
                {
                    assetName = storyText.Substring(assetStartIndex + 1);
                }

                storyData.hasCommand = true;
                storyData.commandName = assetName;
                storyData.commandText = assetText;
            }
            else
            {
                characterText = storyText.Substring(characterTextIndex);
            }

            storyData.characterId = id;
            storyData.characterName = characterName;
            storyData.characterText = characterText;

            return storyData;
        }

        private float GetHeightOfTempTextBox(CharacterUIData characterUiData, TextBoxUIValues textBoxUiValues, string storyText)
        {
            tempTextBox.AddTextToDialogue(storyText);

            tempTextBox.SetDialogueAlignment(characterUiData.textBoxAlignment, textBoxUiValues);

            var verticalPadding = textBoxUiValues.verticalPadding + textBoxUiValues.interTextBoxSpacingValue;

            tempTextBox.SetTextBoxVerticalPadding(verticalPadding);

            Canvas.ForceUpdateCanvases();

            return _rectTransform.rect.height;
        }

        private void InstantiateTextBoxObject(DialogueTextBox prefab, CharacterUIData characterUiData, CharacterUIData.TypeOfTextBox typeOfTextBox,
            float height, string storyText, float animationDuration)
        {
            prefab.SetDialogueUI(characterUiData, _textBoxUiValues, height, animationDuration);

            StartCoroutine(SetTextBoxText(prefab, typeOfTextBox, storyText, animationDuration));
        }

        private IEnumerator SetTextBoxText(DialogueTextBox textBox, CharacterUIData.TypeOfTextBox typeOfTextBox, string storyText, float animationDuration)
        {
            yield return new WaitForSeconds(animationDuration);

            textBox.InstantiateDialogue(storyText, typeOfTextBox, animationDuration);
        }

        public OptionButtonTextBox GenerateOptionForThisNode(string optionText)
        {
            var storyData = GetStoryData(optionText);

            var characterUiData =
                characterUIDataScriptableObject.characterUIDatas[int.Parse(storyData.characterId) - 1];

            var height = GetHeightOfTempTextBox(characterUiData, _textBoxUiValues, storyData.characterText);

            var optionDialogueGameObject = Instantiate(optionButtonPrefab, storyTextParent);

            optionDialogueGameObject.ThisStoryData = storyData;

            InstantiateTextBoxObject(optionDialogueGameObject, characterUiData, CharacterUIData.TypeOfTextBox.Option, height, storyData.characterText, _textBoxFadeInDuration);

            _optionButtonTextBoxes.Add(optionDialogueGameObject);

            return optionDialogueGameObject;
        }

        public void DestroyRemainingOptionDialoguesOnSelection(int chosenOptionId)
        {
            var optionDialogue = _optionButtonTextBoxes[chosenOptionId];

            if (optionDialogue.ThisStoryData.hasCommand)
            {
                StartCoroutine(HandleCommand(optionDialogue.ThisStoryData));
            }

            for (var i = 0; i < _optionButtonTextBoxes.Count; i++)
            {
                _optionButtonTextBoxes[i].ToggleOptionVisibility(false);
                _optionButtonTextBoxes[i].ClearText();
                _optionButtonTextBoxes[i].dialogueButton.interactable = false;

                if (i == 0)
                {
                    var height = GetHeightOfTempTextBox(optionDialogue.ThisCharacterUIData, _textBoxUiValues, optionDialogue.Dialogue);

                    InstantiateTextBoxObject(_optionButtonTextBoxes[i], optionDialogue.ThisCharacterUIData, CharacterUIData.TypeOfTextBox.Dialogue, height, optionDialogue.Dialogue, _optionButtonFadeDuration);
                }
            }

            // Remove the first Option Dialogue as it's already been used for the selection
            _optionButtonTextBoxes.RemoveAt(0);
        }
    }
}
