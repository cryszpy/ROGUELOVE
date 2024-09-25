using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttackType {
    DISTANCE, CLOSE, SPECIAL
}

public class EnemyAttackRangedBurst : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public Weapon parent;

    [SerializeField] private Enemy enemy;

    [SerializeField] protected bool charging;

    [SerializeField] protected PlayerController player;

    [SerializeField] protected CameraShake shake;

    [Header("STATS")]

    public float attackChance;

    public EnemyAttackType attackType;

    [SerializeField] protected float timeBetweenBulletBurst;

    [SerializeField] protected float numberOfBurstShots;

    protected bool bursting = false;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeFrequency;

    // Start is called before the first frame update
    void Start()
    {
        charging = false;
    }

    /* // Update is called once per frame
    public virtual void FixedUpdate()
    {
        FiringMethod();
    } */

    public virtual void FiringMethod() {
        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER && GameStateManager.GetState() != GAMESTATE.MENU 
            && enemy.enemyType != EnemyType.DEAD) {
            
            // If the enemy can fire, sees the player, and is not charging a shot—
            if (!charging && !bursting) {

                // If enemy needs to charge shot, then charge
                if (enemy.lineRenderer != null) {
                    
                    // Starts drawing the line of fire in FixedUpdate()
                    charging = true;
                    StartCoroutine(ChargeShot());
                } 
                // Otherwise just fire
                else {
                    StartCoroutine(BurstFire());
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
    public virtual IEnumerator ChargeShot() {

        // Waits for the charging time
        yield return new WaitForSeconds(enemy.chargeTime);

        // Disables the line sprite renderer
        enemy.lineSpriteRenderer.enabled = false;

        // Disables drawing of line in FixedUpdate()
        charging = false;

        // Fires and stops drawing the line of fire
        StartCoroutine(BurstFire());
    }

    // Firing logic
    public virtual IEnumerator BurstFire()
    {
        if (enemy.enemyType != EnemyType.DEAD) {

            bursting = true;
            enemy.canFire = false;

            for (int i = 0; i < numberOfBurstShots; i++) {

                enemy.animator.SetBool("Attack", true);

                // Play firing sound
                if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                    FireSound();
                }

                GameObject instantBullet = Instantiate(parent.ammo, transform.position, Quaternion.identity);
                StartCoroutine(BulletDestroy(2, instantBullet));

                yield return new WaitForSeconds(timeBetweenBulletBurst);
            }

            // If the enemy doesn't have 0 ranged cooldown, then use min/max values to randomize the next cooldown
            if (enemy.rangedAttackCooldownMin != 0 && enemy.rangedAttackCooldownMax != 0) {
                enemy.attackCooldown = Random.Range(enemy.rangedAttackCooldownMin, enemy.rangedAttackCooldownMax);
            }
        
            bursting = false;
        }
        
    }

    public virtual void FireSound() {
        FindFirstObjectByType<AudioManager>().Play(parent.fireSound);
    }

    // Destroy bullet if it doesn't hit an obstacle and keeps traveling after some time
    public virtual IEnumerator BulletDestroy(float waitTime, GameObject obj) {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            DestroyImmediate(obj, true);
        }
    }
}
