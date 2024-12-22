using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialogueChoice")]
public class OldChoice : DialoguePiece
{

    [Tooltip("The text it says on the choice button.")]
    [TextArea(3, 10)]
    public string choiceText;
}