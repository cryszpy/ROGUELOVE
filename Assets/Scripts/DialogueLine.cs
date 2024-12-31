using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)] public string sentence;

    public Character character;

    public CharacterEmotion emotion;
}
