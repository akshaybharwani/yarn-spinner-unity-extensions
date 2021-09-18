using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace YarnSpinnerUnityExtensions
{
    public struct AssetData
    {
        public string assetName;
        public string assetId;
        public string assetText;
    }

    public enum TypeOfMedia
    {
        Image,
        BackgroundAudioSource,
        OneShotAudioSource
    }

    public class StoryHandler : MonoBehaviour
    {
        [Header("DATA")]
        [SerializeField] private CharacterUIDataScriptableObject characterUIDataScriptableObject = null;
        [SerializeField] private UIOptionsDataScriptableObject UIOptionsDataScriptableObject = null;

        [Header("STORY SCENE OBJECTS")]
        [SerializeField] private Transform storyTextParent = null;
        [SerializeField] private DialogueTextBox tempTextBox = null;
        [SerializeField] private DialogueTextBox dialogueTextBoxPrefab = null;
        [SerializeField] private OptionButtonTextBox optionButtonPrefab = null;
        [SerializeField] private StoryImageBox storyImageBox = null;

        [Header("EXTERNAL ASSETS")]
        [SerializeField] private string imagesResourcesPath = "Images";
        [SerializeField] private string audioClipsResourcesPath = "AudioClips";

        private const string MediaIdentifier = "-";

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
        }

        public void HandleNextNode(string storyText)
        {
            var assetData = GetAssetData(storyText);

            if (assetData.assetId == MediaIdentifier)
            {
                HandleMedia(assetData);
            }
            else
            {
                HandleStoryText(assetData);
            }
        }

        private void HandleMedia(AssetData mediaData)
        {
            switch (mediaData.assetName.Trim())
            {
                case nameof(TypeOfMedia.Image):
                    HandleImage(mediaData);
                    break;
            }
        }

        private void HandleImage(AssetData mediaData)
        {
            var sprite = Resources.Load<Sprite>(imagesResourcesPath + "/" + mediaData.assetText.Trim());
            var height = sprite.texture.height;
            var imageBox = Instantiate(storyImageBox, storyTextParent);
            imageBox.InstantiateStoryImageBox(sprite, _textBoxFadeInDuration, height);
        }

        private void HandleStoryText(AssetData storyTextData)
        {
            var characterUiData =
                characterUIDataScriptableObject.characterUIDatas[int.Parse(storyTextData.assetId) - 1];

            var height = GetHeightOfTempTextBox(characterUiData, _textBoxUiValues, storyTextData.assetText);

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

            InstantiateTextBoxObject(dialogueTextBox, characterUiData, CharacterUIData.TypeOfTextBox.Dialogue, height, storyTextData.assetText, _textBoxFadeInDuration);
        }

        private AssetData GetAssetData(string storyText)
        {
            var id = storyText.Substring(0, 1);

            var assetNameIndex = storyText.IndexOf(":", StringComparison.Ordinal);

            var assetName = storyText.Substring(1,  assetNameIndex - 1);

            var assetText = storyText.Substring(assetNameIndex + 2);

            var assetData = new AssetData {assetText = assetText, assetId = id, assetName = assetName};

            return assetData;
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
            var storyData = GetAssetData(optionText);

            var characterUiData =
                characterUIDataScriptableObject.characterUIDatas[int.Parse(storyData.assetId) - 1];

            var height = GetHeightOfTempTextBox(characterUiData, _textBoxUiValues, storyData.assetText);

            var optionDialogueGameObject = Instantiate(optionButtonPrefab, storyTextParent);

            InstantiateTextBoxObject(optionDialogueGameObject, characterUiData, CharacterUIData.TypeOfTextBox.Option, height, storyData.assetText, _textBoxFadeInDuration);

            _optionButtonTextBoxes.Add(optionDialogueGameObject);

            return optionDialogueGameObject;
        }

        public void DestroyRemainingOptionDialoguesOnSelection(int chosenOptionId)
        {
            var optionDialogue = _optionButtonTextBoxes[chosenOptionId];

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
