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

    protected bool usingAmmo = false;
    protected float ammoTimer = 0;

    public override void FixedUpdate()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            
            Cooldown();

            // Firing logic, if not on cooldown and mouse button pressed, fire
            if (Input.GetMouseButton(0) && parent.currentAmmo > 0) {

                // Use ammo
                usingAmmo = true;

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

    public virtual void Update() {

        // If weapon does not have infinite ammo, use ammo while firing
        if (usingAmmo && !parent.infiniteAmmo) {
            ammoTimer += Time.deltaTime;

            if (ammoTimer > parent.fireRate * PlayerController.FireRateMultiplier) {
                usingAmmo = false;
                ammoTimer = 0;

                UseAmmo();
            }
        }
    }

    // Weapon fire cooldown
    public override void Cooldown() {
        
        if (!canFire) {
            timer += Time.deltaTime;
            
            if(timer > parent.fireRate * PlayerController.FireRateMultiplier) {
                canFire = true;
                timer = 0;
            }
        }
    }

    public override void UseAmmo()
    {
        base.UseAmmo();
        
        // Start camera shake
        TriggerCamShake();
    }

    public virtual void EnableVisualSpread(bool value) {
        coll.enabled = value;

        var emission = particleField.emission;
        
        switch (value) {
            case true:

                // Play firing sound
                if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                    FireSound();
                }
                
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
            }
        }
    }

    public virtual IEnumerator FlameFire(EnemyHealth script) {

        for (int i = 0; i < numberOfBurstShots; i++) {

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
        
        parent.bursting = false;
        canFire = false;
    }
}
