using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PlayerData
{

    public float playerDamageModifier;
    public float playerFireRateModifier;

    public int playerHealth;
    public int playerMaxHealth;

    public float playerMoveSpeed;

    public float experienceLevel;
    public float maxExperienceLevel;

    public int playerCoins;

    public int currentWeaponIndex;
    public int primaryWeaponRarity;
    public int primaryWeaponID;
    public int secondaryWeaponRarity;
    public int secondaryWeaponID;

    public float ammoMaxMultiplier;
    public float primaryWeaponCurrentAmmo;
    public float secondaryWeaponCurrentAmmo;

    public PlayerData (PlayerController player, Weapon weapon) {

        playerDamageModifier = player.damageModifier;

        experienceLevel = PlayerController.Experience;
        maxExperienceLevel = PlayerController.MaxEnergy;

        playerHealth = player.Health;
        playerMaxHealth = PlayerController.MaxHealth;
        playerMoveSpeed = PlayerController.MoveSpeed;
        playerFireRateModifier = player.fireRateModifier;

        playerCoins = PlayerController.Coins;

        currentWeaponIndex = PlayerController.CurrentWeaponIndex;
        primaryWeaponRarity = (int)PlayerController.PrimaryWeaponRarity;
        primaryWeaponID = PlayerController.PrimaryWeaponID;
        secondaryWeaponRarity = (int)PlayerController.SecondaryWeaponRarity;
        secondaryWeaponID = PlayerController.SecondaryWeaponID;

        ammoMaxMultiplier = PlayerController.AmmoMaxMultiplier;
        primaryWeaponCurrentAmmo = PlayerController.PrimaryWeaponCurrentAmmo;
        secondaryWeaponCurrentAmmo = PlayerController.SecondaryWeaponCurrentAmmo;
        
    }
}
