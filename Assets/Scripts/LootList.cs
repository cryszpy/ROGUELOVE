using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponRarity {
    COMMON, UNCOMMON, RARE, EPIC, LEGENDARY
}

[System.Serializable]
public struct ChestAreaRarities {

    public float[] area;
}

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/LootList")]
public class LootList : ScriptableObject
{
    public ChestAreaRarities[] areaRarities;

    public WeaponPair testingWeapon;
    
    [Tooltip("List of all common weapon *pickup* objects.")]
    public List<WeaponPair> commonWeapons;
    public List<WeaponPair> seenCommonWeapons;

    [Tooltip("List of all uncommon weapon *pickup* objects.")]
    public List<WeaponPair> uncommonWeapons;
    public List<WeaponPair> seenUncommonWeapons;

    [Tooltip("List of all rare weapon *pickup* objects.")]
    public List<WeaponPair> rareWeapons;
    public List<WeaponPair> seenRareWeapons;

    [Tooltip("List of all epic weapon *pickup* objects.")]
    public List<WeaponPair> epicWeapons;
    public List<WeaponPair> seenEpicWeapons;

    [Tooltip("List of all legendary weapon *pickup* objects.")]
    public List<WeaponPair> legendaryWeapons;
    public List<WeaponPair> seenLegendaryWeapons;

    [Tooltip("List of all special weapon *pickup* objects.")]
    public List<WeaponPair> specialWeapons;
    public List<WeaponPair> seenSpecialWeapons;

    public int drawnWeaponID;

    public WeaponPair GetRandomWeapon(WeaponRarity rarity) {

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

    public void RemoveWeapon(WeaponPair pair, WeaponRarity rarity) {

        switch (rarity) {
            case WeaponRarity.COMMON:
                seenCommonWeapons.Add(pair);
                commonWeapons.Remove(pair);
                break;
            case WeaponRarity.UNCOMMON:
                seenUncommonWeapons.Add(pair);
                uncommonWeapons.Remove(pair);
                break;
            case WeaponRarity.RARE:
                seenRareWeapons.Add(pair);
                rareWeapons.Remove(pair);
                break;
            case WeaponRarity.EPIC:
                seenEpicWeapons.Add(pair);
                epicWeapons.Remove(pair);
                break;
            case WeaponRarity.LEGENDARY:
                seenLegendaryWeapons.Add(pair);
                legendaryWeapons.Remove(pair);
                break;
        }
    }

    public void ResetWeapons(List<WeaponPair> swapFrom, List<WeaponPair> swapTo) {
        if (swapFrom.Count != 0) {
            foreach (WeaponPair pair in swapFrom.ToArray()) {
                swapTo.Add(pair);
                swapFrom.Remove(pair);
            }
        }
    }

    public void ResetAllWeapons() {
        ResetWeapons(seenCommonWeapons, commonWeapons);
        ResetWeapons(seenUncommonWeapons, uncommonWeapons);
        ResetWeapons(seenRareWeapons, rareWeapons);
        ResetWeapons(seenEpicWeapons, epicWeapons);
        ResetWeapons(seenLegendaryWeapons, legendaryWeapons);
    }
}
