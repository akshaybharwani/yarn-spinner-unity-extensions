using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignDialogue(string characterName, string dialogueText, bool alignment)
    {
        _characterNameText.text = characterName;
        _dialogueText.text = dialogueText;

        if (alignment)
        {
            _characterNameText.alignment = TextAlignmentOptions.Left;
            _dialogueText.alignment = TextAlignmentOptions.Left;
        }
        else
        {
            _characterNameText.alignment = TextAlignmentOptions.Right;
            _dialogueText.alignment = TextAlignmentOptions.Right;
        }

    }
}
