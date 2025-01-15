using System.Collections;
using UnityEngine;

public class EnemyAttackRadius : MonoBehaviour
{

    // Parent class
    [SerializeField]
    protected Enemy parent;
    
    public virtual void OnTriggerEnter2D (Collider2D collider) {

        // If collided with the player, start attack sequence
        if (collider.CompareTag("Player") && parent.enemyType != EnemyType.DEAD) {

            // Disable collider and animation trigger to prevent looping
            parent.contactColl.enabled = false;

            // Tell parent script that player has been hit with a melee attack (inside contact collider)
            parent.inContactColl = true;

            // Damage entity
            StartCoroutine(AttackEntity(collider));

        } 
    }

    public virtual void OnTriggerExit2D (Collider2D collider) {

        // If collided with the player, tell parent script that player has left contact collider
        if (collider.CompareTag("Player")) {

            // Tell parent enemy that player has been hit with a melee attack (inside contact collider)
            parent.inContactColl = false;

        } 
    }

    public virtual IEnumerator AttackEntity(Collider2D target)
    {
        // Deal damage to enemy                        
        if (target.TryGetComponent<PlayerController>(out var player)) {

            // Start animation
            parent.animator.SetBool("Attack", true);

            player.TakeDamage(parent.contactDamage);
            
        } else {
            Debug.LogError("Tried to damage nonexistent entity! Or the entity has no collider.");
        }
        //attackAnim = true;

        // Wait for attack cooldown
        yield return new WaitForSeconds(parent.attackCooldown);

        parent.contactColl.enabled = true;
    }
}
