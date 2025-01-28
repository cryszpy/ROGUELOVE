using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class WeaponSingleShotFire : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    private Camera mainCam;

    public Weapon parent;

    [SerializeField] private PlayerController player;

    [SerializeField] protected CinemachineImpulseSource camShake;

    [Header("STATS")]

    public bool canFire = true;
    protected float timer;

    [SerializeField] private float shakeAmplitude = 1f;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public virtual void FixedUpdate()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {

            Cooldown();

            // Firing logic, if not on cooldown and mouse button pressed, fire
            if (Input.GetMouseButton(0) && canFire && parent.currentAmmo > 0) {
                Fire();

                // If weapon doesn't have infinite ammo then use ammo
                if (!parent.infiniteAmmo) {
                    UseAmmo();
                }
            }
        }
    }

    // Weapon fire cooldown
    public virtual void Cooldown() {
        
        if (!canFire) {
            timer += Time.fixedDeltaTime;
            
            if(timer > parent.fireRate * PlayerController.FireRateMultiplier) {
                canFire = true;
                timer = 0;
            }
        }
    }

    // Firing logic
    public virtual void Fire() {
        
        canFire = false;

        // Play firing sound
        if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
            FireSound();
        }

        // Start camera shake
        if (camShake == null) {
            camShake = GetComponent<CinemachineImpulseSource>();
            Debug.Log("CinemachineImpulseSource camShake is null! Reassigned.");
        }

        // Empty GameObject for chosen bullet
        GameObject chosenBullet = null;

        // If weapon has multiple possible ammo bullets—
        if (parent.ammoList.Count > 1) {

            // Checks to make sure all ammo scripts are accessible
            foreach (var ammoStruct in parent.ammoList) {
                if (!ammoStruct.ammo.TryGetComponent<BulletScript>(out var script)) {
                    Debug.LogError("Could not find BulletScript script or extension of such on this Object.");
                }
            }

            // Picks a random projectile to spawn
            float rand = UnityEngine.Random.value;

            // Loops through all possible ammo to compare spawn thresholds
            foreach (var ammoStruct in parent.ammoList) {

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
        else if (parent.ammoList.Count == 1) {

            chosenBullet = parent.ammoList[0].ammo;
        }

        // Spawns bullet
        SpawnBullet(chosenBullet);
    }

    public virtual GameObject SpawnBullet(GameObject ammo) {

        // Spawn bullet and add player damage modifier
        if (ammo.TryGetComponent<BulletScript>(out var bullet)) {
            GameObject instantBullet = bullet.Create(ammo, parent.spawnPos.transform.position, Quaternion.identity, parent, mainCam) as GameObject;

            // Play muzzle flash animation
            if (parent.spawnPos.TryGetComponent<Animator>(out var animator)) {
                animator.SetTrigger("MuzzleFlash");
            }

            // Destroy bullet after 2 seconds
            StartCoroutine(BulletDestroy(2, instantBullet));

            return instantBullet;
        } else {
            Debug.LogError("Could not find BulletScript script or extension of such on this Object.");
            return null;
        }
    }

    public virtual void FireSound() {
        AudioManager.instance.PlaySoundByName(parent.fireSound, parent.spawnPos.transform);
    }

    public virtual void UseAmmo() {

        // Start camera shake
        TriggerCamShake();

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

    public virtual void TriggerCamShake() {

        if (camShake == null) {
            camShake = GetComponent<CinemachineImpulseSource>();
            Debug.Log("CinemachineImpulseSource camShake is null! Reassigned.");
        }

        Vector3 mousePos = GameStateManager.ToWorldPoint(Input.mousePosition, mainCam);

        Vector3 direction = mousePos - transform.position;

        camShake.GenerateImpulse(direction.normalized * shakeAmplitude);
    }

    // Destroy bullet if it doesn't hit an obstacle and keeps traveling after some time
    public virtual IEnumerator BulletDestroy(float waitTime, GameObject obj) {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            DestroyImmediate(obj, true);
        }
    }
}
