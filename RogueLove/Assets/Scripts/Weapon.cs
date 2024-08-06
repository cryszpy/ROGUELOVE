using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    MAGIC, TECHNOLOGY, SPECIAL
}

public class Weapon : MonoBehaviour
{

    [Tooltip("The weapon's type.")]
    public WeaponType type;

    [Tooltip("The weapon's rarity.")]
    public WeaponRarity rarity;

    public int id;

    [Tooltip("The object reference for this weapon's projectile object.")]
    public GameObject ammo;

    [Tooltip("The maximum ammo capacity of this weapon.")]
    public float ammoMax;

    // Used ammo per click
    public float ammoPerClick;

    // Current amount of ammo
    public float currentAmmo;

    // Infinite ammo boolean
    public bool infiniteAmmo;

    // Fire rate / cooldown
    public float timeBetweenFiring;

    [Tooltip("The worth of this weapon when sold, in coins.")]
    public float coinValue;

    [Tooltip("Flavor text that appears on screen when a weapon is picked up for the first time.")]
    public string flavorText;

    // Weapon sprite
    public SpriteRenderer sprite;

    // Weapon fire sound
    public string fireSound;

    // Weapon charge sound
    public string chargeSound;

    // Bullet Spawn Point
    public GameObject spawnPos;

    public Vector2 distance;

}