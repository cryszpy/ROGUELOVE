using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/DialogueList")]
public class DialogueList : ScriptableObject
{
    public string id_prefix;

    // List of all available dialogue without prerequisites in the game.
    public List<Dialogue> noRequirements;

    // List of all unavailable dialogue without prerequisites in the game.
    public List<Dialogue> seenNoRequirements;



    // List of all available casual dialogue with prerequisites in the game.
    public List<Dialogue> requirements;

    // List of all unavailable casual dialogue with prerequisites in the game.
    public List<Dialogue> seenRequirements;

}