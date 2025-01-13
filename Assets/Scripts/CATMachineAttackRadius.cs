using UnityEngine;

public class CATMachineAttackRadius : EnemyAttackRadius
{
    
    public override void OnTriggerEnter2D(Collider2D collider) {

        // If collided with the player, start attack sequence
        if (collider.CompareTag("Player") && parent.enemyType != EnemyType.DEAD) {

            // Disable collider and animation trigger to prevent looping
            parent.contactColl.enabled = false;

            // Damage entity
            StartCoroutine(AttackEntity(collider));

        } 
    }
}
