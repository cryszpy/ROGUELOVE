using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponRarity {
    COMMON, UNCOMMON, RARE, EPIC, LEGENDARY
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/LootList")]
public class LootList : ScriptableObject
{

    public float[] areaOne;
    public float[] areaTwo;
    public float[] areaThree;
    public float[] areaFour;
    public float[] areaFive;
    public float[] areaSix;
    public float[] areaSeven;
    public float[] areaEight;
    
    [Tooltip("List of all common weapon *pickup* objects.")]
    public List<GameObject> commonWeapons;
    public List<GameObject> seenCommonWeapons;

    [Tooltip("List of all uncommon weapon *pickup* objects.")]
    public List<GameObject> uncommonWeapons;
    public List<GameObject> seenUncommonWeapons;

    [Tooltip("List of all rare weapon *pickup* objects.")]
    public List<GameObject> rareWeapons;
    public List<GameObject> seenRareWeapons;

    [Tooltip("List of all epic weapon *pickup* objects.")]
    public List<GameObject> epicWeapons;
    public List<GameObject> seenEpicWeapons;

    [Tooltip("List of all legendary weapon *pickup* objects.")]
    public List<GameObject> legendaryWeapons;
    public List<GameObject> seenLegendaryWeapons;

    public List<GameObject> items;
    public List<GameObject> seenItems;

    public GameObject GetRandomWeapon(WeaponRarity rarity) {

        int rand;
        switch (rarity) {
            case WeaponRarity.COMMON:
                rand = UnityEngine.Random.Range(0, commonWeapons.Count - 1);
                return commonWeapons[rand];
            case WeaponRarity.UNCOMMON:
                rand = UnityEngine.Random.Range(0, uncommonWeapons.Count - 1);
                return uncommonWeapons[rand];
            case WeaponRarity.RARE:
                rand = UnityEngine.Random.Range(0, rareWeapons.Count - 1);
                return rareWeapons[rand];
            case WeaponRarity.EPIC:
                rand = UnityEngine.Random.Range(0, epicWeapons.Count - 1);
                return epicWeapons[rand];
            case WeaponRarity.LEGENDARY:
                rand = UnityEngine.Random.Range(0, legendaryWeapons.Count - 1);
                return legendaryWeapons[rand];
            default:
                rand = UnityEngine.Random.Range(0, uncommonWeapons.Count - 1);
                return uncommonWeapons[rand];
        }
    }

    public void RemoveWeapon(GameObject weapon, WeaponRarity rarity) {

        switch (rarity) {
            case WeaponRarity.COMMON:
                seenCommonWeapons.Add(weapon);
                commonWeapons.Remove(weapon);
                break;
            case WeaponRarity.UNCOMMON:
                seenUncommonWeapons.Add(weapon);
                uncommonWeapons.Remove(weapon);
                break;
            case WeaponRarity.RARE:
                seenRareWeapons.Add(weapon);
                rareWeapons.Remove(weapon);
                break;
            case WeaponRarity.EPIC:
                seenEpicWeapons.Add(weapon);
                epicWeapons.Remove(weapon);
                break;
            case WeaponRarity.LEGENDARY:
                seenLegendaryWeapons.Add(weapon);
                legendaryWeapons.Remove(weapon);
                break;
        }
    }

    public void ResetWeapons(List<GameObject> swapFrom, List<GameObject> swapTo) {
        foreach (GameObject weapon in swapFrom.ToArray()) {
            swapTo.Add(weapon);
            swapFrom.Remove(weapon);
        }
    }

    public GameObject GetRandomItem() {
        int rand = UnityEngine.Random.Range(0, items.Count - 1);
        return items[rand];
    }

    public void RemoveItem(GameObject item) {
        seenItems.Add(item);
        items.Remove(item);
    }

    public void ResetItems(List<GameObject> swapFrom, List<GameObject> swapTo) {
        foreach (GameObject item in swapFrom.ToArray()) {
            swapTo.Add(item);
            swapFrom.Remove(item);
        }
    }
}
