using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletPiercingScript : BulletScript
{

    // Damage enemies or destroy self when hitting obstacles
    public override void OnTriggerEnter2D(Collider2D other) {
        
        direction = mousePos - transform.position;

        // Checks whether collided object is an enemy
        if (other.CompareTag("Enemy")) {

            // Tries to get the EnemyHealth component of collided enemy
            if (other.TryGetComponent<EnemyHealth>(out var enemy)) {

                // If bullet is a flame bullet, deal fire damage
                if (isFire && !enemy.immuneToFire) {
                    enemy.TakeFireDamage(damage, direction);
                }
                else {
                    enemy.TakeDamage(damage, direction);
                }
            }
            else {
                Debug.LogError("Could not get collided enemy's EnemyHealth component!");
            }
        }

        // If collided object layer is not player (3) or breakables (10) or enemies (9)
        if (other.gameObject.layer != 3 && other.gameObject.layer != 10 && other.gameObject.layer != 9) {
            coll.enabled = false;
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }
}
