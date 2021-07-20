using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryHandler : MonoBehaviour
{
    [SerializeField] private Transform _storyTextParent;
    [SerializeField] private StoryDialogue _storyDialoguePrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateStoryDialogueForThisText(string storyText)
    {
        if (storyText.Contains("-"))
        {
            var characterNameEndIndex = storyText.IndexOf(":", StringComparison.Ordinal);
            
            //var characterName = storyText.Substring(1,  characterNameEndIndex - 1);

            var dialogueText = storyText.Substring(characterNameEndIndex + 2);

            var storyDialogueGameObject = Instantiate(_storyDialoguePrefab, _storyTextParent);
            
            storyDialogueGameObject.AssignDialogue(dialogueText);
        }
    }
}
