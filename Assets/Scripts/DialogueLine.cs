using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)] public string sentence = null;

    public Character character = null;

    public CharacterEmotion emotion = CharacterEmotion.DEFAULT;
}
