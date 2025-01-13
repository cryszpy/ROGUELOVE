using System.Collections;
using UnityEngine;

public class EnemyAttackProjectileRing : EnemyBulletSpawner
{

    public float rotationSpeed;

    public int ringAmount;

    public float ringCooldown;

    // Start is called before the first frame update
    public override void Start()
    {
        charging = false;
    }

    public virtual void Update() {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + rotationSpeed);
    }

    public override void FiringMethod() {
        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER && GameStateManager.GetState() != GAMESTATE.MENU && enemy.enemyType != EnemyType.DEAD) {
            Debug.Log("1");
            
            // If the enemy can fire, sees the player, and is not charging a shot—
            if (!charging && !bursting) {
                Debug.Log("2");

                // If enemy needs to charge shot, then charge
                if (enemy.lineRenderer != null) {
                    Debug.Log("4");
                    
                    // Starts drawing the line of fire in FixedUpdate()
                    charging = true;
                    StartCoroutine(ChargeShot());
                } 
                // Otherwise just fire
                else {
                    Debug.Log("3");
                    StartAttackAnim();
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

    // Initiate the charging of a shot
    public override IEnumerator ChargeShot() {
        Debug.Log("5");

        // Waits for the charging time
        yield return new WaitForSeconds(enemy.chargeTime);

        // Disables the line sprite renderer
        enemy.lineSpriteRenderer.enabled = false;

        // Disables drawing of line in FixedUpdate()
        charging = false;

        // Fires and stops drawing the line of fire
        StartAttackAnim();
    }

    // Firing logic
    public override void StartAttackAnim() {
        if (enemy.enemyType != EnemyType.DEAD && GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            Debug.Log("STARTATTACKANIM()");

            bursting = true;
            enemy.canFire = false;

            enemy.animator.SetBool("Attack", true);
        }
    }

    // Firing logic
    public override IEnumerator Attack() {
        Debug.Log("ATTACK()");

        // Shoots specified number of rings
        for (int i = 0; i < ringAmount; i++) {
            
            for (int b = 0; b < numberOfBurstShots; b++) {

                // Play firing sound
                if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                    FireSound();
                }

                // Spawns bullet
                GameObject instantBullet = Instantiate(parent.ammo, transform.position, Quaternion.identity);
                StartCoroutine(BulletDestroy(2, instantBullet));
                if (instantBullet.TryGetComponent<EnemyBulletScript>(out var bullet)) {
                    bullet.transform.rotation = transform.rotation;
                }
                /* if (parent.ammo.TryGetComponent<EnemyBulletScript>(out var bullet)) {
                    GameObject projectile = (GameObject)bullet.Create(parent.ammo, transform.position, Quaternion.identity, gameObject);
                    StartCoroutine(BulletDestroy(2, projectile));
                    projectile.transform.rotation = transform.rotation;
                } else {
                    Debug.LogError("Could not find EnemyBulletScript component on this object!");
                } */

                yield return new WaitForSeconds(timeBetweenBulletBurst);
            }

            yield return new WaitForSeconds(ringCooldown);

        }

        // If the enemy doesn't have 0 ranged cooldown, then use min/max values to randomize the next cooldown
        if (enemy.rangedAttackCooldownMin != 0 && enemy.rangedAttackCooldownMax != 0) {
            enemy.attackCooldown = Random.Range(enemy.rangedAttackCooldownMin, enemy.rangedAttackCooldownMax);
        }
    
        enemy.currentAttack = null;
        bursting = false;
        enemy.animator.SetBool("Attack", false);
    }
}
