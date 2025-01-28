using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    MAGIC, TECHNOLOGY, SPECIAL
}

[System.Serializable]
public struct AmmoStruct {

    public GameObject ammo;

    [Header("The float value that corresponds to the spawn chance for this bullet. (Is affected by other bullet chances, all bullets must add up to 1)")]
    [Range(0, 1)]
    public float spawnChanceCutoff;
}

public class Weapon : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    [Tooltip("This weapon's ammo.")]
    public List<AmmoStruct> ammoList;

    [Tooltip("Weapon sprite.")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("Spawn position of the bullet.")]
    public GameObject spawnPos;

    [Tooltip("This weapon's pickup item variant.")]
    public GameObject weaponPickup;

    [Header("STATS")]

    [Tooltip("The weapon's type.")]
    public WeaponType type;

    [Tooltip("The weapon's rarity.")]
    public WeaponRarity rarity;

    [Tooltip("This weapon's ID number (found in spreadsheet).")]
    public int id;

    [Tooltip("The maximum ammo capacity of this weapon.")]
    public float ammoMax;

    [Tooltip("The amount of ammo this weapon uses per click.")]
    public float ammoPerClick;

    [Tooltip("The current ammo count of this weapon.")]
    public float currentAmmo;

    [Tooltip("Whether this weapon has infinite ammo or not.")]
    public bool infiniteAmmo;

    [Tooltip("How long it takes to charge this weapon.")]
    public float chargeTime;

    [Tooltip("The cooldown time between firing bullets.")]
    public float fireRate;

    [Tooltip("The worth of this weapon when sold, in coins.")]
    public float coinValue;

    [Tooltip("Flavor text that appears on screen when a weapon is picked up for the first time.")]
    public string flavorText;

    [Tooltip("The sound this weapon makes when firing.")]
    public string fireSound;

    [Tooltip("The sound this weapon makes when charging.")]
    public string chargeSound;

    [Tooltip("The sound this weapon's bullets make when hitting something.")]
    public string bulletHitSound;

    [HideInInspector] public bool bursting = false;

}