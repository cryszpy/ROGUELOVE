using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Character")]
public class Character : ScriptableObject
{

    public List<CharacterExpression> expressions;

    public string characterName;

    public string characterNameHidden;

    public bool nameRevealed;
}

[System.Serializable]
public struct CharacterExpression {

    public CharacterEmotion emotion;

    public Sprite expressionSprite; 
}

public enum CharacterEmotion {
    DEFAULT, HAPPY, ANGRY, EMBARRASSED, ANXIOUS, SAD, SHOCKED
}