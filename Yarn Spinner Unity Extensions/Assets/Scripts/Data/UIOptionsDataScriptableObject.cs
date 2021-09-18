using System;
using UnityEngine;
[CreateAssetMenu(fileName = "UIOptionsDataScriptableObject", menuName = "ScriptableObjects/UIOptionsData", order = 1)]
public class UIOptionsDataScriptableObject : ScriptableObject
{
    public UIOptionsData UIOptionsData;
    [Header("Text Box Padding Values")]
    public TextBoxUIValues textBoxUIValues;
}

[Serializable]
public class UIOptionsData
{
    public float optionButtonFadeDuration;
    public float textBoxFadeInDuration = 1f;
    public float dialogueAfterChoiceDuration;
}

[Serializable]
public class TextBoxUIValues
{
    public int interTextBoxSpacingValue;
    public int centerHorizontalMargin;
    public int minHorizontalMargin;
    public int maxHorizontalMargin;
    public int verticalMargin;
    public int horizontalPadding;
    public int verticalPadding;
}
