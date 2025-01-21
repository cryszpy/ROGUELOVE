using System.Collections;
using UnityEngine;

public class EnemyContactAttack : EnemyAttackBase
{

    [Tooltip("This attack's contact damage.")]
    public int attackDamage = 1;

    // Called from Enemy script when first using the attack
    public override void FiringMethod() {
        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER && GameStateManager.GetState() != GAMESTATE.MENU 
            && parent.enemyType != EnemyType.DEAD) {
            
            StartAttackAnim();
        }
    }

    // Starts the attack animation and bullet firing
    public override void StartAttackAnim() {

        if (parent.enemyType != EnemyType.DEAD && GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            parent.canFire = false;

            parent.animator.SetBool("Attack", true);
        }
    }

    // Called from SpawnAttack() which is called in attack animation
    public override IEnumerator Attack() {
        parent.attacking = true;

        // Damage player if the player is still inside range
        if (parent.inContactColl) {

            // Damage player
            parent.player.TakeDamage(attackDamage);
        }

        // Wait for attack cooldown
        yield return new WaitForSeconds(parent.attackCooldown);

        // Reset attacks
        parent.currentAttack = null;
        parent.attacking = false;
    }
    
    public virtual void OnTriggerEnter2D (Collider2D collider) {

        // If collided with the player, start attack sequence
        if (collider.CompareTag("Player") && parent.enemyType != EnemyType.DEAD && !parent.inContactColl) {

            // Tell parent script that player has been hit with a melee attack (inside contact collider)
            parent.inContactColl = true;
        } 
    }

    public virtual void OnTriggerExit2D (Collider2D collider) {

        // If collided with the player, tell parent script that player has left contact collider
        if (collider.CompareTag("Player") && parent.inContactColl) {

            // Tell parent enemy that player has been hit with a melee attack (inside contact collider)
            parent.inContactColl = false;
        } 
    }
}
