using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CATMachineAttackRadius : EnemyAttackRadius
{
    
    public override void OnTriggerEnter2D(Collider2D collider) {

        // If collided with the player, start attack sequence
        if (collider.CompareTag("Player")) {

            // Disable collider and animation trigger to prevent looping
            parent.contactColl.enabled = false;

            // Damage entity
            StartCoroutine(AttackEntity(collider));

        } 
    }
}
