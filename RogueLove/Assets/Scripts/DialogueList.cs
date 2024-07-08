using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/DialogueList")]
public class DialogueList : ScriptableObject
{

    public bool movedDialogue = false;

    // List of queued dialogue
    public List<Dialogue> priority;

    public List<Dialogue> GetDialogue(DialogueType type) {
        return type switch
        {
            DialogueType.NOREQ => noRequirements,
            DialogueType.REQ => requirements,
            DialogueType.QUEST => questCalls,
            DialogueType.SUPPORT => supportCalls,
            DialogueType.SHOP => shopCalls,
            DialogueType.TRADE => tradeCalls,
            DialogueType.STORYLINE => storyline,
            _ => priority,
        };
    }

    public List<Dialogue> GetSeenDialogue(DialogueType type) {
        return type switch
        {
            DialogueType.NOREQ => seenNoRequirements,
            DialogueType.REQ => seenRequirements,
            DialogueType.QUEST => seenQuestCalls,
            DialogueType.SUPPORT => seenSupportCalls,
            DialogueType.SHOP => seenShopCalls,
            DialogueType.TRADE => seenTradeCalls,
            DialogueType.STORYLINE => seenStoryline,
            _ => priority,
        };
    }

    public void DiscardDialogue(Dialogue dialogue, DialogueType type) {

        priority.Add(dialogue);

        switch (type) {
            case DialogueType.NOREQ:
                seenNoRequirements.Add(dialogue);
                noRequirements.Remove(dialogue);
                break;
            case DialogueType.REQ:
                seenRequirements.Add(dialogue);
                requirements.Remove(dialogue);
                break;
            case DialogueType.QUEST:
                seenQuestCalls.Add(dialogue);
                questCalls.Remove(dialogue);
                break;
            case DialogueType.SUPPORT:
                seenSupportCalls.Add(dialogue);
                supportCalls.Remove(dialogue);
                break;
            case DialogueType.SHOP:
                seenShopCalls.Add(dialogue);
                shopCalls.Remove(dialogue);
                break;
            case DialogueType.TRADE:
                seenTradeCalls.Add(dialogue);
                tradeCalls.Remove(dialogue);
                break;
            case DialogueType.STORYLINE:
                seenStoryline.Add(dialogue);
                storyline.Remove(dialogue);
                break;
        }
        movedDialogue = true;
    }

    public void ResetDialogue(List<Dialogue> swapFrom, List<Dialogue> swapTo) {
        foreach (Dialogue dialogue in swapFrom.ToArray()) {
            swapTo.Add(dialogue);
            swapFrom.Remove(dialogue);
        }
        ShuffleList(swapTo);
    }

    public void ShuffleList(List<Dialogue> list) {

        for(int i = list.Count - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    // List of all available dialogue without prerequisites in the game.
    [SerializeField]
    private List<Dialogue> noRequirements;

    // List of all unavailable dialogue without prerequisites in the game.
    [SerializeField]
    private List<Dialogue> seenNoRequirements;



    // List of all available casual dialogue with prerequisites in the game.
    [SerializeField]
    private List<Dialogue> requirements;

    // List of all unavailable casual dialogue with prerequisites in the game.
    [SerializeField]
    private List<Dialogue> seenRequirements;



    // List of all available quest call dialogue in the game.
    [SerializeField]
    private List<Dialogue> questCalls;

    // List of all unavailable quest call dialogue in the game.
    [SerializeField]
    private List<Dialogue> seenQuestCalls;

    

    // List of all available support call dialogue in the game.
    [SerializeField]
    private List<Dialogue> supportCalls;

    // List of all unavailable support call dialogue in the game.
    [SerializeField]
    private List<Dialogue> seenSupportCalls;



    // List of all available shop call dialogue in the game.
    [SerializeField]
    private List<Dialogue> shopCalls;

    // List of all unavailable shop call dialogue in the game.
    [SerializeField]
    private List<Dialogue> seenShopCalls;



    // List of all available trade call dialogue in the game.
    [SerializeField]
    private List<Dialogue> tradeCalls;

    // List of all unavailable quest call dialogue in the game.
    [SerializeField]
    private List<Dialogue> seenTradeCalls;



    // List of all available storyline dialogue in the game.
    [SerializeField]
    private List<Dialogue> storyline;

    // List of all unavailable storyline dialogue in the game.
    [SerializeField]
    private List<Dialogue> seenStoryline;

}

// DIALOGUE TYPES
public enum DialogueType {
    NOREQ, REQ, QUEST, SUPPORT, SHOP, TRADE, STORYLINE
}