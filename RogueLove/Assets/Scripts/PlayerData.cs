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

    public PlayerData (PlayerController player, PlayerAim weapon) {

        if (weapon.bullet.TryGetComponent<BulletScript>(out var bull)) {
            playerDamage = bull.damage;
        }

        experienceLevel = PlayerController.GetExperience();
        maxExperienceLevel = PlayerController.GetMaxEnergy();

        playerHealth = player.Health;
        playerMaxHealth = PlayerController.GetMaxPlayerHealth();
        playerMoveSpeed = PlayerController.GetMoveSpeed();
        playerAttackSpeed = weapon.timeBetweenFiring;
        
    }
}
