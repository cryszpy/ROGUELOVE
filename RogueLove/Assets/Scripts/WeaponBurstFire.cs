using System.Collections;
using UnityEngine;

public class WeaponBurstFire : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public Weapon parent;

    [SerializeField] private PlayerController player;

    [Header("STATS")]

    public bool canFire = true;
    protected float timer;

    protected bool bursting = false;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER) {
            
            Cooldown();

            Fire();
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

    // Firing logic
    public virtual void Fire() {
        // Firing logic, if not on cooldown and mouse button pressed, fire
        if (Input.GetMouseButton(0) && canFire && !bursting) {
            bursting = true;
            StartCoroutine(BurstFire());
        }
    }

    public virtual IEnumerator BurstFire() {

        if (parent.ammo.TryGetComponent<BulletScript>(out var bullet)) {
            bullet.damage *= player.damageModifier;
        }

        if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
            FireSound();
        }

        GameObject instantBullet = Instantiate(parent.ammo, parent.spawnPos.transform.position, Quaternion.identity);
        StartCoroutine(BulletDestroy(2, instantBullet));

        yield return new WaitForSeconds(0.15f);

        if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
            FireSound();
        }

        GameObject instantBullet1 = Instantiate(parent.ammo, parent.spawnPos.transform.position, Quaternion.identity);
        StartCoroutine(BulletDestroy(2, instantBullet1));

        yield return new WaitForSeconds(0.15f);

        if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
            FireSound();
        }

        GameObject instantBullet2 = Instantiate(parent.ammo, parent.spawnPos.transform.position, Quaternion.identity);
        StartCoroutine(BulletDestroy(2, instantBullet2));
        //StartCoroutine(shake.Shake(shakeDuration, shakeAmplitude, shakeFrequency));
        //camShake.Shake(0.15f, 0.4f);
        bursting = false;
        canFire = false;
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
