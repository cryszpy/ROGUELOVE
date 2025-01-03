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
    public float playerFireRateMult;

    public int playerHealth;
    public int playerMaxHealth;

    public float playerMoveSpeed;
    public float moveSpeedMult;

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

    public int heldItemsCount;
    public int[] heldItemsID;
    public ItemRarity[] heldItemsRarities;

    public float dodgeChance;
    public float takenDamageMult;
    public float viewRangeBase;
    public float viewRangeMult;

    public float bigChestChance;

    public float fireDamageMultiplier;
    public float baseFireDamage;

    public PlayerData (PlayerController player, Weapon weapon) {

        playerDamageModifier = PlayerController.DamageModifier;

        experienceLevel = PlayerController.Experience;
        maxExperienceLevel = PlayerController.MaxEnergy;

        playerHealth = player.Health;
        playerMaxHealth = PlayerController.MaxHealth;

        playerMoveSpeed = PlayerController.MoveSpeed;
        moveSpeedMult = PlayerController.MoveSpeedMultiplier;

        playerFireRateMult = PlayerController.FireRateMultiplier;
        
        fireDamageMultiplier = PlayerController.FireDamageMultiplier;
        baseFireDamage = PlayerController.BaseFireDamage;

        playerCoins = PlayerController.Coins;

        currentWeaponIndex = PlayerController.CurrentWeaponIndex;
        primaryWeaponRarity = (int)PlayerController.PrimaryWeaponRarity;
        primaryWeaponID = PlayerController.PrimaryWeaponID;
        secondaryWeaponRarity = (int)PlayerController.SecondaryWeaponRarity;
        secondaryWeaponID = PlayerController.SecondaryWeaponID;

        ammoMaxMultiplier = PlayerController.AmmoMaxMultiplier;
        primaryWeaponCurrentAmmo = PlayerController.PrimaryWeaponCurrentAmmo;
        secondaryWeaponCurrentAmmo = PlayerController.SecondaryWeaponCurrentAmmo;

        heldItemsCount = PlayerController.HeldItemsCount;
        heldItemsID = new int[heldItemsCount];
        heldItemsRarities = new ItemRarity[heldItemsCount];
        for (int i = 0; i < heldItemsCount; i++) {
            heldItemsID[i] = PlayerController.HeldItemsID[i];
            heldItemsRarities[i] = PlayerController.HeldItemsRarity[i];
        }

        dodgeChance = PlayerController.DodgeChance;
        takenDamageMult = PlayerController.TakenDamageMult;
        viewRangeMult = PlayerController.ViewRangeMultiplier;
        viewRangeBase = PlayerController.ViewRangeBase;

        bigChestChance = PlayerController.BigChestChance;
    }
}
