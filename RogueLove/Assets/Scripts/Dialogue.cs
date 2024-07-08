using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject
{

    public string id;

    public Sprite characterSprite;

    public string characterName;

    public bool seenBefore;

    public int storyline;

    public int storylineScene;

    public bool question;

    public bool choice;

    public Dialogue[] choices;

    [TextArea(3, 10)]
    public string[] sentences;

}
