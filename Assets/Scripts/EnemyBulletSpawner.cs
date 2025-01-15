using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyAttackType {
    DISTANCE, CLOSE, SPECIAL
}

public class EnemyBulletSpawner : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public Weapon parent;

    [SerializeField] protected Enemy enemy;

    [Header("STATS")]

    public float attackChance;

    public EnemyAttackType attackType;

    [SerializeField] protected float timeBetweenBulletBurst;

    [SerializeField] protected float numberOfBurstShots;

    protected bool isChargedShot = false;

    [SerializeField] protected float chargeTimer;

    public virtual void Update() {

        // If enemy is charging a shot—
        if (enemy.charging) {

            // If something blocks line of sight while charging, reset charge and wait
            if (!enemy.hitPlayer) {
                enemy.charging = false;
                chargeTimer = 0;
                enemy.bursting = false;
                isChargedShot = false;
                enemy.canFire = false;
                enemy.currentAttack = null;
                TurnOffCharge();
                return;
            } 

            chargeTimer += Time.deltaTime;

            // Aim towards the player
            if (enemy.enemyType != EnemyType.BOSS) {
                RotateSpawner();
            }

            // Enable the line renderer if it isn't already
            if (enemy.lineSpriteRenderer.enabled == false && enemy.lineSpriteRenderer != null) {
                enemy.lineSpriteRenderer.enabled = true;
            }

            // Draw the line of fire
            if (enemy.lineRenderer != null) {
                enemy.lineRenderer.PointA = enemy.transform.position;
                enemy.lineRenderer.PointB = enemy.player.transform.position;
            }

            // Once charging is complete—
            if (chargeTimer > enemy.chargeTime) {

                // Reset
                enemy.charging = false;
                chargeTimer = 0;

                // Start firing
                StartCoroutine(FinishedCharging());
            }
        } 
        // If enemy isn't charging, reset cooldown
        else{
            chargeTimer = 0;

            // If the enemy also isn't firing anything, disable line renderer
            if (!enemy.bursting) {
                TurnOffCharge();
            }
        }
    }

    // Aims this weapon at the player
    public virtual void RotateSpawner() {

        // Sets the rotation of the spawner to face the player right before firing
        Vector3 rotation =  enemy.player.transform.position - enemy.transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    // Disables the line renderer if this enemy charges
    public virtual void TurnOffCharge() {
        if (enemy.lineSpriteRenderer && enemy.lineRenderer) {
            enemy.lineSpriteRenderer.enabled = false;
            enemy.lineRenderer.SetColorA(Color.red);
            enemy.lineRenderer.SetColorB(Color.red);
        }
    }

    public virtual void FiringMethod() {
        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER && GameStateManager.GetState() != GAMESTATE.MENU 
            && enemy.enemyType != EnemyType.DEAD) {
            
            // If the enemy isn't charging a shot or in the middle of an attack and still sees the player—
            if (!enemy.charging && !enemy.bursting && enemy.hitPlayer) {

                // If enemy needs to charge shot, then charge
                if (enemy.lineRenderer != null) {
                    
                    // Starts drawing the line of fire in Update()
                    isChargedShot = true;
                    enemy.charging = true;
                } 
                // Otherwise just fire
                else {
                    StartAttackAnim();
                }
            }
        }
    }

    // Initiate the charging of a shot
    public virtual IEnumerator FinishedCharging() {
        enemy.bursting = true;
        enemy.charging = false;

        // Flash to signal firing
        enemy.lineRenderer.SetColorA(Color.white);
        enemy.lineRenderer.SetColorB(Color.white);

        yield return new WaitForSeconds(0.08f);

        enemy.lineRenderer.SetColorA(Color.yellow);
        enemy.lineRenderer.SetColorB(Color.yellow);

        yield return new WaitForSeconds(0.08f);

        enemy.lineRenderer.SetColorA(Color.white);
        enemy.lineRenderer.SetColorB(Color.white);

        yield return new WaitForSeconds(0.08f);

        // Disables drawing of line in Update()
        TurnOffCharge();

        // Fires and stops drawing the line of fire
        StartAttackAnim();
    }

    // Starts the attack animation and bullet firing
    public virtual void StartAttackAnim() {

        if (enemy.enemyType != EnemyType.DEAD && GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            enemy.canFire = false;

            enemy.animator.SetBool("Attack", true);
        }
    }

    // Called from Animation Event once attack animation is complete, fires actual attack
    public virtual void SpawnAttack() {
        StartCoroutine(Attack());
    }

    public virtual IEnumerator Attack() {
        
        // If the enemy doesn't need to charge, make sure the bullet aims towards the 
        // last known player position immediately before firing.
        if (!isChargedShot) {
            RotateSpawner();
        }

        // For the specified number of bullets in this burst attack—
        for (int i = 0; i < numberOfBurstShots; i++) {

            // Play firing sound
            if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                FireSound();
            }

            // Spawns bullet
            GameObject instantBullet = Instantiate(parent.ammo, transform.position, Quaternion.identity);
            StartCoroutine(BulletDestroy(2, instantBullet));

            // Sets bullet rotation aimed at player
            if (instantBullet.TryGetComponent<EnemyBulletScript>(out var bullet)) {
                bullet.transform.rotation = transform.rotation;
            }

            // Waits for specified amount of time between bullets in burst
            yield return new WaitForSeconds(timeBetweenBulletBurst);
        }

        // If the enemy doesn't have 0 ranged cooldown, then use min/max values to randomize the next cooldown
        if (enemy.rangedAttackCooldownMin != 0 && enemy.rangedAttackCooldownMax != 0) {
            enemy.attackCooldown = Random.Range(enemy.rangedAttackCooldownMin, enemy.rangedAttackCooldownMax);
        }

        // Reset attacks
        enemy.currentAttack = null;
        enemy.bursting = false;
        enemy.animator.SetBool("Attack", false);
        isChargedShot = false;
    }

    public virtual void FireSound() {
        AudioManager.instance.PlaySoundByName(parent.fireSound, parent.spawnPos.transform);
    }

    // Destroy bullet if it doesn't hit an obstacle and keeps traveling after some time
    public virtual IEnumerator BulletDestroy(float waitTime, GameObject obj) {
        yield return new WaitForSeconds(waitTime);
        Destroy(obj);
    }
}
