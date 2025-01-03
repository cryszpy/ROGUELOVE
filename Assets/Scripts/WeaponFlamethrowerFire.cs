using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFlamethrowerFire : WeaponBurstFire
{
    public float damage;

    public float knockback;

    public bool doesFireDamage;

    [SerializeField] protected Collider2D coll;
    [SerializeField] protected GameObject particleSys;
    [SerializeField] protected ParticleSystem particleField;

    protected List<EnemyHealth> collidedEnemies = new();

    public override void FixedUpdate()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            
            Cooldown();

            // Firing logic, if not on cooldown and mouse button pressed, fire
            if (Input.GetMouseButton(0) && parent.currentAmmo > 0) {

                // Enables flamethrower detection radius
                EnableVisualSpread(true);

                // Damage enemies within radius
                /* if (canFire && !bursting) {

                    // If gun needs to charge, begin charging
                    if (parent.chargeTime > 0) {
                        chargeTimer += Time.fixedDeltaTime;

                        // Once charging is finished, begin firing
                        if (chargeTimer >= parent.chargeTime && collidedEnemies.Count > 0) {
                            bursting = true;
                            StartCoroutine(BurstFire());

                            // If weapon doesn't have infinite ammo then use ammo
                            if (!parent.infiniteAmmo) {
                                UseAmmo();
                            }
                        }
                    } 
                    // Otherwise, fire
                    else if (collidedEnemies.Count > 0) {
                        bursting = true;
                        StartCoroutine(BurstFire());

                        // If weapon doesn't have infinite ammo then use ammo
                        if (!parent.infiniteAmmo) {
                            UseAmmo();
                        }
                    }
                } */
            } else {
                EnableVisualSpread(false);
            }

            // Reset charging
            if (!Input.GetMouseButton(0) && chargeTimer > 0) {
                chargeTimer = 0;
                EnableVisualSpread(false);
            }
        }
    }

    public virtual void EnableVisualSpread(bool value) {
        coll.enabled = value;

        var emission = particleField.emission;
        
        switch (value) {
            case true:
                
                emission.rateOverTime = 100;
                break;
            case false:
                emission.rateOverTime = 0;
                break;
        }
        //particleSys.SetActive(value);
    }

    public virtual void OnParticleCollision(GameObject other) {
        
        if (other.CompareTag("Enemy") && other.TryGetComponent<EnemyHealth>(out var script)) {
            
            if (canFire) {
                StartCoroutine(FlameFire(script));

                // If weapon doesn't have infinite ammo then use ammo
                if (!parent.infiniteAmmo) {
                    UseAmmo();
                }
            }
        }
    }

    public virtual IEnumerator FlameFire(EnemyHealth script) {
        
        // Assign camera shake
        if (shake == null) {
            Debug.Log("CameraShake camShake is null! Reassigned.");
            //shake = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraShake>();
            shake = player.hurtShake;
        }

        for (int i = 0; i < numberOfBurstShots; i++) {

            // Play firing sound
            if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                FireSound();
            }

            // Start camera shake
            StartCoroutine(shake.Shake(shakeDuration, shakeAmplitude, shakeFrequency));
            //camShake.Shake(0.15f, 0.4f);

            switch (doesFireDamage) {
                case true:
                    script.TakeFireDamage(damage, script.transform.position - transform.position, knockback);
                    break;
                case false:
                    script.TakeDamage(damage, script.transform.position - transform.position, knockback);
                    break;
            }

            yield return new WaitForSeconds(timeBetweenBulletBurst);
        }
        
        bursting = false;
        canFire = false;
    }

    // Adds enemies to collided list if in range
    /* public virtual void OnTriggerEnter2D (Collider2D collider) {

        if (collider.CompareTag("Enemy") && collider.gameObject.TryGetComponent<EnemyHealth>(out var script)) {
            if (!collidedEnemies.Contains(script)) {
                collidedEnemies.Add(script);
            }
        }
    }

    // Removes enemies from collided list if out of range
    public virtual void OnTriggerExit2D (Collider2D collider) {

        if (collider.CompareTag("Enemy") && collider.gameObject.TryGetComponent<EnemyHealth>(out var script)) {
            if (collidedEnemies.Contains(script)) {
                collidedEnemies.Remove(script);
            }
        }
    } */
}
