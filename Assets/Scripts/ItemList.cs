using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemRarity {
    COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, SPECIAL, FLOWER
}

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ItemList")]
public class ItemList : ScriptableObject
{
    public ChestAreaRarities[] areaRarities;
    
    [Tooltip("List of all common item pickups.")]
    public List<ItemPickup> commonItems;
    public List<ItemPickup> seenCommonItems;

    [Tooltip("List of all uncommon item pickups.")]
    public List<ItemPickup> uncommonItems;
    public List<ItemPickup> seenUncommonItems;

    [Tooltip("List of all rare item pickups.")]
    public List<ItemPickup> rareItems;
    public List<ItemPickup> seenRareItems;

    [Tooltip("List of all epic item pickups.")]
    public List<ItemPickup> epicItems;
    public List<ItemPickup> seenEpicItems;

    [Tooltip("List of all legendary item pickups.")]
    public List<ItemPickup> legendaryItems;
    public List<ItemPickup> seenLegendaryItems;

    [Tooltip("List of all special item pickups.")]
    public List<ItemPickup> specialItems;
    public List<ItemPickup> seenSpecialItems;

    [Tooltip("List of all flower item pickups.")]
    public List<ItemPickup> flowerItems;
    public List<ItemPickup> seenFlowerItems;

    public int drawnItemID;

    public ItemPickup GetRandomItem(ItemRarity rarity) {
        int rand;
        switch (rarity) {
            case ItemRarity.COMMON:
                rand = UnityEngine.Random.Range(0, commonItems.Count - 1);
                return commonItems[rand];
            case ItemRarity.UNCOMMON:
                rand = UnityEngine.Random.Range(0, uncommonItems.Count - 1);
                return uncommonItems[rand];
            case ItemRarity.RARE:
                rand = UnityEngine.Random.Range(0, rareItems.Count - 1);
                return rareItems[rand];
            case ItemRarity.EPIC:
                rand = UnityEngine.Random.Range(0, epicItems.Count - 1);
                return epicItems[rand];
            case ItemRarity.LEGENDARY:
                rand = UnityEngine.Random.Range(0, legendaryItems.Count - 1);
                return legendaryItems[rand];
            case ItemRarity.SPECIAL:
                rand = UnityEngine.Random.Range(0, specialItems.Count - 1);
                return specialItems[rand];
            case ItemRarity.FLOWER:
                rand = UnityEngine.Random.Range(0, flowerItems.Count - 1);
                return flowerItems[rand];
        }
        Debug.LogWarning("Item rarity not found!");
        return null;
    }

    public void RemoveItem(ItemPickup item, ItemRarity rarity) {
        switch (rarity) {
            case ItemRarity.COMMON:
                seenCommonItems.Add(item);
                commonItems.Remove(item);
                break;
            case ItemRarity.UNCOMMON:
                seenUncommonItems.Add(item);
                uncommonItems.Remove(item);
                break;
            case ItemRarity.RARE:
                seenRareItems.Add(item);
                rareItems.Remove(item);
                break;
            case ItemRarity.EPIC:
                seenEpicItems.Add(item);
                epicItems.Remove(item);
                break;
            case ItemRarity.LEGENDARY:
                seenLegendaryItems.Add(item);
                legendaryItems.Remove(item);
                break;
            case ItemRarity.SPECIAL:
                seenSpecialItems.Add(item);
                specialItems.Remove(item);
                break;
            case ItemRarity.FLOWER:
                seenFlowerItems.Add(item);
                flowerItems.Remove(item);
                break;
        }
    }

    public void ResetItems(List<ItemPickup> swapFrom, List<ItemPickup> swapTo) {
        if (swapFrom.Count != 0) {
            foreach (ItemPickup item in swapFrom.ToArray()) {
                swapTo.Add(item);
                swapFrom.Remove(item);
            }
        }
    }

    public void ResetAllItems() {
        ResetItems(seenCommonItems, commonItems);
        ResetItems(seenUncommonItems, uncommonItems);
        ResetItems(seenRareItems, rareItems);
        ResetItems(seenEpicItems, epicItems);
        ResetItems(seenLegendaryItems, legendaryItems);
        ResetItems(seenSpecialItems, specialItems);
        ResetItems(seenFlowerItems, flowerItems);
    }
}
