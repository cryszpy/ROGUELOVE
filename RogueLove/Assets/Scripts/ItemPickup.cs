using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* [System.Serializable]
public class ItemPair {
    public GameObject pickupObject;
    public ItemPickup pickupScript;
    public float dropChance;
} */

public enum ItemType {
    STAT, ABILITY, ACTIVE, FLOWER
}

public class ItemPickup : BasePickup, IPickupable
{
    public int itemID;

    public float dropChance;

    public ItemEffect effect;

    [Tooltip("Item sprite.")]
    public Sprite imageSprite;

    [Tooltip("The item's type.")]
    public ItemType type;

    [Tooltip("The item's rarity.")]
    public ItemRarity rarity;

    [Tooltip("The worth of this item when sold, in coins.")]
    public float coinValue;

    [Tooltip("Description text that appears in the dictionary?")]
    public string descText;

    [Tooltip("Flavor text that appears on screen when an item is picked up for the first time.")]
    public string flavorText;

    // Item pickup sound
    public string pickupSound;

    public override void Update() {

        if (!playerFound) {
            return;
        } 
        // Pick up item
        else if (Input.GetKeyDown(KeyCode.E) && playerFound && playerInRadius) {
            Pickup(false);
        }
    }

    public void Pickup(bool replace) {

        player.heldItems.Add(this); // Adds item to player's held items list

        // If this is the first time picking up the item ever, add it to the new items list and trigger UI animation
        if (!HomeManager.SeenItems.Contains(itemID)) {
            HomeManager.SeenItems.Add(itemID);
            HomeManager.SeenItemsCount++;
            GameStateManager.pickupManager.StartAnimation();
            PlayerController.EOnNewPickup?.Invoke();
        }

        PlayerController.HeldItemsID.Add(this.itemID);
        PlayerController.HeldItemsRarity.Add(this.rarity);
        PlayerController.HeldItemsCount++;

        PlayerController.EOnItemPickup?.Invoke();

        effect.OnPickup(); // Triggers the pickup effect, if possible

        Debug.Log("Picked up " + this.name + "!");
        RemoveObject();
    }

    public override void RemoveObject()
    {
        gameObject.SetActive(false);
    }

}
