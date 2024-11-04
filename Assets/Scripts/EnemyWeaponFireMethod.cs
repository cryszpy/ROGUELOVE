using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponFireMethod : WeaponSingleShotFire
{
    [SerializeField] private Enemy enemy;

    [SerializeField] private bool charging;

    // Start is called before the first frame update
    void Start()
    {
        canFire = false;
        charging = false;
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER && GameStateManager.GetState() != GAMESTATE.MENU 
            && enemy.enemyType != EnemyType.DEAD) {

            Cooldown();
            
            // If the enemy can fire, sees the player, and is not charging a shot—
            if (canFire && enemy.inFollowRadius && enemy.hitPlayer && enemy.seen && !charging) {

                // If enemy needs to charge shot, then charge
                if (enemy.lineRenderer != null) {
                    
                    // Starts drawing the line of fire in FixedUpdate()
                    charging = true;
                    StartCoroutine(ChargeShot());
                } 
                // Otherwise just fire
                else {
                    Fire();
                }
            }

            // If enemy is charging a shot—
            if (charging) {

                // Enable the line renderer if it isn't already
                if (enemy.lineSpriteRenderer.enabled == false && enemy.lineSpriteRenderer != null) {
                    enemy.lineSpriteRenderer.enabled = true;
                }

                // Draw the line of fire
                if (enemy.lineRenderer != null) {
                    enemy.lineRenderer.PointA = enemy.transform.position;
                    enemy.lineRenderer.PointB = enemy.player.transform.position;
                }
            }
        }
    }

    // Firing cooldown timer
    public override void Cooldown()
    {
        if (!canFire) {
            timer += Time.fixedDeltaTime;

            if(timer > enemy.attackCooldown) {
                canFire = true;
                timer = 0;
            }
        }
    }

    // Initiate the charging of a shot
    public virtual IEnumerator ChargeShot() {

        // Waits for the charging time
        yield return new WaitForSeconds(enemy.chargeTime);

        // Disables the line sprite renderer
        enemy.lineSpriteRenderer.enabled = false;

        // Disables drawing of line in FixedUpdate()
        charging = false;

        // Fires and stops drawing the line of fire
        Fire();
    }

    // Firing logic
    public override void Fire()
    {
        if (enemy.enemyType != EnemyType.DEAD) {
            
            enemy.animator.SetBool("Attack", true);

            canFire = false;

            enemy.attackCooldown = Random.Range(enemy.rangedAttackCooldownMin, enemy.rangedAttackCooldownMax);

            if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                FireSound();
            }

            GameObject instantBullet = Instantiate(parent.ammo, transform.position, Quaternion.identity);
            StartCoroutine(BulletDestroy(2, instantBullet));
        }
        
    }
}
