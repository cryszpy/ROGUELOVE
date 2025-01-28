using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct EnemyAttackStruct {

    public EnemyAttackBase attack;

    [Tooltip("This attack's chance of being used. (MUST ALL ADD UP TO 1)")]
    [Range(0, 1)]
    public float attackChance;
}

public class EnemyAttackBase : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public Weapon weapon;

    [SerializeField] protected Enemy parent;

    // Called by the Enemy script when starting an attack
    public virtual void FiringMethod() {
        Debug.LogWarning("Base FiringMethod() called!");
        return;
    }

    // Starts the attack animation and bullet firing
    public virtual void StartAttackAnim() {

        if (parent.enemyType != EnemyType.DEAD && GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            parent.canFire = false;

            parent.animator.SetBool("Attack", true);
        }
    }

    // Called from Animation Event once attack animation is complete, fires actual attack
    public virtual void SpawnAttack() {
        StartCoroutine(Attack());
    }

    public virtual IEnumerator Attack() {
        Debug.LogWarning("Base Attack() coroutine called!");
        yield break;
    }

    public virtual void FireSound() {
        AudioManager.instance.PlaySoundByName(weapon.fireSound, weapon.spawnPos.transform);
    }

    public virtual GameObject SpawnBullet(GameObject ammo) {

        // Spawn bullet
        GameObject instantBullet = Instantiate(ammo, transform.position, Quaternion.identity);

        // Destroy bullet after 2 seconds
        StartCoroutine(BulletDestroy(2, instantBullet));

        return instantBullet;
    }

    // Destroy bullet if it doesn't hit an obstacle and keeps traveling after some time
    public virtual IEnumerator BulletDestroy(float waitTime, GameObject obj) {
        yield return new WaitForSeconds(waitTime);
        Destroy(obj);
    }
}
