using System.Collections;
using UnityEngine;

public class EnemyRingAttack : EnemyRangedAttack
{

    public float rotationSpeed;

    public int ringAmount;

    public float ringCooldown;

    public float ringRotation;

    public override void Update() {
        base.Update();
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + rotationSpeed);
    }

    // Firing logic
    public override IEnumerator Attack() {
        parent.attacking = true;

        // If the enemy doesn't need to charge, make sure the bullet aims towards the 
        // last known player position immediately before firing.
        if (!isChargedShot) {
            RotateSpawner();
        }

        // Shoots specified number of rings
        for (int i = 0; i < ringAmount; i++) {

            // Angle rings by rotation angle
            if (i > 0) {
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + ringRotation);
            }
            
            // For the specified number of bullets in this burst attack—
            for (int b = 0; b < numberOfBurstShots; b++) {

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

            yield return new WaitForSeconds(ringCooldown);

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
