using System.Collections;
using UnityEngine;

public class WeaponBurstFire : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public Weapon parent;

    [SerializeField] private CameraShake shake;

    [SerializeField] private PlayerController player;

    [Header("STATS")]

    public bool canFire = true;
    protected float timer;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeFrequency;

    protected bool bursting = false;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER && GameStateManager.GetState() != GameStateManager.GAMESTATE.MENU) {
            
            Cooldown();

            // Firing logic, if not on cooldown and mouse button pressed, fire
            if (Input.GetMouseButton(0) && canFire && parent.currentAmmo > 0 && !bursting) {
                bursting = true;
                StartCoroutine(BurstFire());

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
            timer += Time.deltaTime;
            
            if(timer > parent.timeBetweenFiring) {
                canFire = true;
                timer = 0;
            }
        }
    }

    public virtual IEnumerator BurstFire() {

        // Add player damage modifier
        if (parent.ammo.TryGetComponent<BulletScript>(out var bullet)) {
            bullet.damage *= player.damageModifier;
        }

        if (shake == null) {
            Debug.Log("CameraShake camShake is null! Reassigned.");
            //shake = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraShake>();
            shake = player.hurtShake;
        }

        if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
            FireSound();
        }

        // Start camera shake
        StartCoroutine(shake.Shake(shakeDuration, shakeAmplitude, shakeFrequency));
        //camShake.Shake(0.15f, 0.4f);

        GameObject instantBullet = Instantiate(parent.ammo, parent.spawnPos.transform.position, Quaternion.identity);
        StartCoroutine(BulletDestroy(2, instantBullet));

        yield return new WaitForSeconds(0.15f);

        if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
            FireSound();
        }

        // Start camera shake
        StartCoroutine(shake.Shake(shakeDuration, shakeAmplitude, shakeFrequency));
        //camShake.Shake(0.15f, 0.4f);

        GameObject instantBullet1 = Instantiate(parent.ammo, parent.spawnPos.transform.position, Quaternion.identity);
        StartCoroutine(BulletDestroy(2, instantBullet1));

        yield return new WaitForSeconds(0.15f);

        if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
            FireSound();
        }

        // Start camera shake
        StartCoroutine(shake.Shake(shakeDuration, shakeAmplitude, shakeFrequency));
        //camShake.Shake(0.15f, 0.4f);

        GameObject instantBullet2 = Instantiate(parent.ammo, parent.spawnPos.transform.position, Quaternion.identity);
        StartCoroutine(BulletDestroy(2, instantBullet2));
        
        bursting = false;
        canFire = false;
    }

    public virtual void FireSound() {
        FindFirstObjectByType<AudioManager>().Play(parent.fireSound);
    }

    public virtual void UseAmmo() {
        if (parent.currentAmmo - parent.ammoPerClick < 0) {
            parent.currentAmmo = 0;
        }
        else {
            parent.currentAmmo -= parent.ammoPerClick;
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
