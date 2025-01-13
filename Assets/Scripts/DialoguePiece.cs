using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueChoice {

    [TextArea(2, 5)] public string choiceText;

    public DialogueLine[] lines;
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Dialogue")]
public class DialoguePiece : ScriptableObject, System.IComparable
{

    public string id;

    public Character owner;

    [Tooltip("This dialogue piece's total sentences.")]
    public DialogueNode[] nodes;

    public int priority;

    public List<DialogueRequirement> requirements;

    public CallDialogueType callDialogueType;

    public int CompareTo(object obj) {
        var a = this;
        var b = obj as DialoguePiece;

        if (a.priority > b.priority) {
            return -1;
        }

        if (a.priority > b.priority) {
            return 1;
        }

        return 0;
    }

}

[System.Serializable]
public struct DialogueRequirement {

    public DialogueRequirementType reqType;

    public int targetObjectID;

    public string statReqToCheck;
}

public enum DialogueRequirementType {
    NONE, 
    IS_HOLDING, 
    IS_CARRYING, 
    HAS_TALKED_TO, 
    HAS_KILLED, 
    STAGE_NUM, 
    LEVEL_NUM, 
    ENERGY_NUM, 
    HEALTH_NUM, 
    SPEED_NUM, 
    HAS_DIED, 
    HAS_NOT_OPENED_CHEST, 
    IS_ON_FIRE
}

public enum CallDialogueType {
    NONE, DEFAULT, SUPPORT, QUEST, SHOP, TRADE
}