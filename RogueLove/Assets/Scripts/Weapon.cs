using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Type of bullet to be fired
    public GameObject ammo;

    // Max ammo capacity of the weapon
    public float ammoMax;

    // Used ammo per click
    public float ammoPerClick;

    // Current amount of ammo
    public float currentAmmo;

    // Infinite ammo boolean
    public bool infiniteAmmo;

    // Fire rate / cooldown
    public float timeBetweenFiring;

    // Weapon sprite
    public SpriteRenderer sprite;

    // Weapon fire sound
    public string fireSound;

    // Weapon charge sound
    public string chargeSound;

    // Bullet Spawn Point
    public GameObject spawnPos;
}