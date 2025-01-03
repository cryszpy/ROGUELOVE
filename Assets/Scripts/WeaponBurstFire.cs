using System.Collections;
using UnityEngine;

public class WeaponBurstFire : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    protected Camera mainCam;

    public Weapon parent;

    [SerializeField] protected CameraShake shake;

    [SerializeField] protected PlayerController player;

    [Header("STATS")]

    [SerializeField] protected float timeBetweenBulletBurst;

    [SerializeField] protected float numberOfBurstShots;

    public bool canFire = true;
    protected float timer;

    [SerializeField] protected float shakeDuration;
    [SerializeField] protected float shakeAmplitude;
    [SerializeField] protected float shakeFrequency;

    protected bool bursting = false;

    protected float chargeTimer;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            
            Cooldown();

            // Firing logic, if not on cooldown and mouse button pressed, fire
            if (Input.GetMouseButton(0) && canFire && parent.currentAmmo > 0 && !bursting) {

                // If gun needs to charge, begin charging
                if (parent.chargeTime > 0) {
                    chargeTimer += Time.fixedDeltaTime;

                    // Once charging is finished, begin firing
                    if (chargeTimer >= parent.chargeTime) {
                        bursting = true;
                        StartCoroutine(BurstFire());

                        // If weapon doesn't have infinite ammo then use ammo
                        if (!parent.infiniteAmmo) {
                            UseAmmo();
                        }
                    }
                } 
                // Otherwise, fire
                else {
                    bursting = true;
                    StartCoroutine(BurstFire());

                    // If weapon doesn't have infinite ammo then use ammo
                    if (!parent.infiniteAmmo) {
                        UseAmmo();
                    }
                }
            }

            // Reset charging
            if (!Input.GetMouseButton(0) && chargeTimer > 0) {
                chargeTimer = 0;
            }
        }
    }

    // Weapon fire cooldown
    public virtual void Cooldown() {
        
        if (!canFire) {
            timer += Time.deltaTime;
            
            if(timer > parent.fireRate * PlayerController.FireRateMultiplier) {
                canFire = true;
                timer = 0;
            }
        }
    }

    public virtual IEnumerator BurstFire() {
        
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

            // Spawn bullet and add player damage modifier
            if (parent.ammo.TryGetComponent<BulletScript>(out var bullet)) {
                GameObject instantBullet = bullet.Create(parent.ammo, parent.spawnPos.transform.position, Quaternion.identity, parent, mainCam) as GameObject;

                // Play muzzle flash animation
                if (parent.spawnPos.TryGetComponent<Animator>(out var animator)) {
                    animator.SetTrigger("MuzzleFlash");
                }

                // Destroy bullet after 2 seconds
                StartCoroutine(BulletDestroy(2, instantBullet));
            } else {
                Debug.LogError("Could not find BulletScript script or extension of such on this Object.");
            }

            yield return new WaitForSeconds(timeBetweenBulletBurst);
        }
        
        bursting = false;
        canFire = false;
    }

    public virtual void FireSound() {
        AudioManager.instance.PlaySoundByName(parent.fireSound, parent.spawnPos.transform);
    }

    public virtual void UseAmmo() {

        // Prevents ammo from going negative
        if (parent.currentAmmo - parent.ammoPerClick < 0) {
            parent.currentAmmo = 0;
        }
        // Reduces current ammo by ammoPerClick amount
        else {
            parent.currentAmmo -= parent.ammoPerClick;
            player.ammoBar.SetAmmo(parent.currentAmmo, parent);
        }

        // Set static current ammo variables according to what slot this weapon is in
        if (PlayerController.CurrentWeaponIndex == 0) {
            if (player.weapon.id == parent.id) {
                PlayerController.PrimaryWeaponCurrentAmmo = parent.currentAmmo;
            }
        } else if (PlayerController.CurrentWeaponIndex == 1) {
            if (player.weapon.id == parent.id) {
                PlayerController.SecondaryWeaponCurrentAmmo = parent.currentAmmo;
            }
        }
    }

    // Destroy bullet if it doesn't hit an obstacle and keeps traveling after some time
    public virtual IEnumerator BulletDestroy(float waitTime, GameObject obj) {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            DestroyImmediate(obj, true);
        }
    }
}
