using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignDialogue(string dialogueText)
    {
        _dialogueText.text = dialogueText;
    }
}
