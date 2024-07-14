using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChestRarity {
    COMMON, UNCOMMON, RARE, EPIC, LEGENDARY
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ChestList")]
public class ChestList : ScriptableObject
{
    
    public GameObject weaponChest;

    public GameObject itemChest;
    
}
