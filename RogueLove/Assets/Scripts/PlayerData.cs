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

    public int playerHealth;

    public int playerMaxHealth;

    public float playerMoveSpeed;

    public float playerFireRateModifier;

    public float experienceLevel;

    public float maxExperienceLevel;

    public PlayerData (PlayerController player, Weapon weapon) {

        playerDamageModifier = player.damageModifier;

        experienceLevel = PlayerController.Experience;
        maxExperienceLevel = PlayerController.MaxEnergy;

        playerHealth = player.Health;
        playerMaxHealth = PlayerController.MaxHealth;
        playerMoveSpeed = PlayerController.MoveSpeed;
        playerFireRateModifier = player.fireRateModifier;
        
    }
}
