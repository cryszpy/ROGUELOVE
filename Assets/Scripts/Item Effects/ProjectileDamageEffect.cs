using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ItemEffect/ProjectileDamageEffect")]
public class ProjectileDamageEffect : ItemEffect 
{
    public float damageModifier;

    public override void OnPickup()
    {
        if (PlayerController.DamageModifier == 1){
            PlayerController.DamageModifier = damageModifier;
        } else {
            PlayerController.DamageModifier += damageModifier;
        }
    }
}