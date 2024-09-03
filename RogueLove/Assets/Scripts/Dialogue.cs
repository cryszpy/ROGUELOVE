using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject, IComparable
{

    public string id;

    public Sprite characterSprite;

    public string characterName;

    public int priority;

    public List<DialogueRequirement> requirements;

    public bool requirementMet;

    public CallDialogueType callDialogueType;

    public bool question;

    public bool choice;

    public Dialogue[] choices;

    public Dialogue[] followUp;

    [TextArea(3, 10)]
    public string[] sentences;

    public int CompareTo(object obj) {
        var a = this;
        var b = obj as Dialogue;

        if (a.priority > b.priority) {
            return -1;
        }

        if (a.priority > b.priority) {
            return 1;
        }

        return 0;
    }

}

[Serializable]
public struct DialogueRequirement {

    public DialogueRequirementType reqType;

    public GameObject objectToFind;

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