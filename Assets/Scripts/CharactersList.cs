using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/CharactersList")]
public class CharactersList : ScriptableObject
{
    public List<Character> allCharacters;

    public void ResetAllCharacterNameReveals() {
        foreach (Character character in allCharacters) {
            character.nameRevealed = false;
        }
    }
}
