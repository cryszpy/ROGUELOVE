using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponFireMethod : WeaponSingleShotFire
{
    [SerializeField] private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        canFire = false;
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER && enemy.enemyType != Enemy.EnemyType.DEAD) {
            
            base.FixedUpdate();
        }
    }

    // Firing cooldown timer
    public override void Cooldown()
    {
        if (!canFire) {
            timer += Time.fixedDeltaTime;

            if(timer > enemy.attackCooldown) {
                canFire = true;
                timer = 0;
            }
        }
    }

    public override void Fire()
    {
        // Firing logic, if not on cooldown and player in range, fire
        if (canFire && enemy.inFollowRadius && enemy.hitPlayer && enemy.seen) {
            enemy.animator.SetBool("Attack", true);
            canFire = false;
            enemy.attackCooldown = Random.Range(enemy.rangedAttackCooldownMin, enemy.rangedAttackCooldownMax);

            if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                FireSound();
            }

            GameObject instantBullet = Instantiate(parent.ammo, transform.position, Quaternion.identity);
            StartCoroutine(BulletDestroy(2, instantBullet));
        }
    }
}
