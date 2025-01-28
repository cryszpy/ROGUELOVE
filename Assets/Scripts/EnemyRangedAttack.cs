using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRangedAttack : EnemyAttackBase
{

    [Header("STATS")]

    [SerializeField] protected float timeBetweenBulletBurst;

    [SerializeField] protected float numberOfBurstShots;

    protected bool isChargedShot = false;

    protected float chargeTimer;

    public virtual void Update() {

        // If enemy is charging a shot—
        if (parent.charging) {

            // If something blocks line of sight while charging, reset charge and wait
            if (!parent.seesPlayer) {
                parent.charging = false;
                chargeTimer = 0;
                parent.attacking = false;
                isChargedShot = false;
                parent.canFire = false;
                parent.currentAttack = null;
                TurnOffCharge();
                return;
            } 

            chargeTimer += Time.deltaTime;

            // Aim towards the player
            if (parent.enemyType != EnemyType.BOSS) {
                RotateSpawner();
            }

            // Enable the line renderer if it isn't already
            if (parent.lineSpriteRenderer.enabled == false && parent.lineSpriteRenderer != null) {
                parent.lineSpriteRenderer.enabled = true;
            }

            // Draw the line of fire
            if (parent.lineRenderer != null) {
                parent.lineRenderer.PointA = parent.transform.position;
                parent.lineRenderer.PointB = parent.player.transform.position;
            }

            // Once charging is complete—
            if (chargeTimer > parent.chargeTime) {

                // Reset
                parent.charging = false;
                chargeTimer = 0;

                // Start firing
                StartCoroutine(FinishedCharging());
            }
        } 
        // If enemy isn't charging, reset cooldown
        else{
            chargeTimer = 0;

            // If the enemy also isn't firing anything, disable line renderer
            if (!parent.attacking) {
                TurnOffCharge();
            }
        }
    }

    // Aims this weapon at the player
    public virtual void RotateSpawner() {

        // Sets the rotation of the spawner to face the player right before firing
        Vector3 rotation =  parent.player.transform.position - parent.transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    // Disables the line renderer if this enemy charges
    public virtual void TurnOffCharge() {
        if (parent.lineSpriteRenderer && parent.lineRenderer) {
            parent.lineSpriteRenderer.enabled = false;
            parent.lineRenderer.SetColorA(Color.red);
            parent.lineRenderer.SetColorB(Color.red);
        }
    }

    public override void FiringMethod() {
        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER && GameStateManager.GetState() != GAMESTATE.MENU 
            && parent.enemyType != EnemyType.DEAD) {
            
            // If the enemy isn't charging a shot or in the middle of an attack and still sees the player—
            if (!parent.charging && !parent.attacking && parent.seesPlayer) {

                // If enemy needs to charge shot, then charge
                if (parent.lineRenderer != null) {
                    
                    // Starts drawing the line of fire in Update()
                    isChargedShot = true;
                    parent.charging = true;
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
        parent.attacking = true;
        parent.charging = false;

        // Flash to signal firing
        parent.lineRenderer.SetColorA(Color.white);
        parent.lineRenderer.SetColorB(Color.white);

        yield return new WaitForSeconds(0.08f);

        parent.lineRenderer.SetColorA(Color.yellow);
        parent.lineRenderer.SetColorB(Color.yellow);

        yield return new WaitForSeconds(0.08f);

        parent.lineRenderer.SetColorA(Color.white);
        parent.lineRenderer.SetColorB(Color.white);

        yield return new WaitForSeconds(0.08f);

        // Disables drawing of line in Update()
        TurnOffCharge();

        // Fires and stops drawing the line of fire
        StartAttackAnim();
    }

    public override IEnumerator Attack() {
        parent.attacking = true;
        
        // If the enemy doesn't need to charge, make sure the bullet aims towards the 
        // last known player position immediately before firing.
        if (!isChargedShot) {
            RotateSpawner();
        }

        // For the specified number of bullets in this burst attack—
        for (int i = 0; i < numberOfBurstShots; i++) {

            // Play firing sound
            if (!string.IsNullOrWhiteSpace(weapon.fireSound)) {
                FireSound();
            }

            // Empty GameObject for chosen bullet
            GameObject chosenBullet = null;

            // If weapon has multiple possible ammo bullets—
            if (weapon.ammoList.Count > 1) {

                // Picks a random projectile to spawn
                float rand = UnityEngine.Random.value;

                // Loops through all possible ammo to compare spawn thresholds
                foreach (var ammoStruct in weapon.ammoList) {

                    // If found chosen bullet, set it as the bullet to spawn and exit loop
                    if (rand <= ammoStruct.spawnChanceCutoff) {
                        chosenBullet = ammoStruct.ammo;
                        break;
                    }
                }

                if (chosenBullet == null) {
                    Debug.LogError("Could not find suitable chosen bullet for this weapon!");
                }
            } 
            // If weapon only has one type of ammo—
            else if (weapon.ammoList.Count == 1) {

                chosenBullet = weapon.ammoList[0].ammo;
            }

            // Use that bullet
            GameObject spawnedBullet = SpawnBullet(chosenBullet);

            // Sets bullet rotation aimed at player
            if (spawnedBullet.TryGetComponent<EnemyBulletScript>(out var bullet)) {
                bullet.transform.rotation = transform.rotation;
            }

            // Waits for specified amount of time between bullets in burst
            yield return new WaitForSeconds(timeBetweenBulletBurst);
        }

        // If the enemy doesn't have 0 ranged cooldown, then use min/max values to randomize the next cooldown
        if (parent.rangedAttackCooldownMin != 0 && parent.rangedAttackCooldownMax != 0) {
            parent.attackCooldown = Random.Range(parent.rangedAttackCooldownMin, parent.rangedAttackCooldownMax);
        }

        // Reset attacks
        parent.currentAttack = null;
        parent.attacking = false;
        parent.animator.SetBool("Attack", false);
        isChargedShot = false;
    }
}
