using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemPair {
    public GameObject pickupObject;
    public ItemPickup pickupScript;
    public float dropChance;
}

public class ItemPickup : BasePickup
{
    public int itemID;
    
}
