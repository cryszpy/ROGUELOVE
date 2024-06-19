using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PlayerData
{

    public float playerDamage;

    public float playerHealth;

    public float playerMaxHealth;

    public float playerMoveSpeed;

    public float playerAttackSpeed;

    public float experienceLevel;

    public float maxExperienceLevel;

    public PlayerData (PlayerController player, Weapon weapon) {

        playerDamage = player.damageModifier;

        experienceLevel = PlayerController.Experience;
        maxExperienceLevel = PlayerController.MaxEnergy;

        playerHealth = player.Health;
        playerMaxHealth = PlayerController.MaxHealth;
        playerMoveSpeed = PlayerController.MoveSpeed;
        playerAttackSpeed = weapon.timeBetweenFiring;
        
    }
}
