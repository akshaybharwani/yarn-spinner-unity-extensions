using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterUIDataScriptableObject", menuName = "ScriptableObjects/CharacterUIData", order = 1)]
public class CharacterUIDataScriptableObject : ScriptableObject
{
    public List<CharacterUIData> characterUIDatas = new List<CharacterUIData>();
}
