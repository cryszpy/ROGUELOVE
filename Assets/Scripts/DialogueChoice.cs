using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DialogueChoice")]
public class DialogueChoice : ScriptableObject
{

    [TextArea(3, 10)]
    public string choiceText;

    public Dialogue nextDialogue;
}