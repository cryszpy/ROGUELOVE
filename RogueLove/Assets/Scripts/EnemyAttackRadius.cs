using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRadius : MonoBehaviour
{

    // Parent class
    [SerializeField]
    private Enemy parent;
    
    private void OnTriggerEnter2D(Collider2D collider) {

        // If collided with the player, start attack sequence
        if (collider.CompareTag("Player")) {

            // Disable collider and animation trigger to prevent looping
            parent.contactColl.enabled = false;

            // Damage entity
            StartCoroutine(AttackEntity(collider));

        } 
    }

    public IEnumerator AttackEntity(Collider2D target)
    {
        // Deal damage to enemy                        
        if (target.TryGetComponent<PlayerController>(out var player)) {

            // Start animation
            parent.animator.SetBool("Attack", true);

            player.TakeDamage(parent.damage);
            
        } else {
            Debug.LogError("Tried to damage nonexistent entity! Or the entity has no collider.");
        }
        //attackAnim = true;

        // Wait for attack cooldown
        yield return new WaitForSeconds(parent.attackCooldown);

        parent.contactColl.enabled = true;
    }
}
