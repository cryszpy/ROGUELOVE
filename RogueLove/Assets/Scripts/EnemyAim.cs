using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAim : PlayerAim
{
    [SerializeField]
    private Enemy parent;

    private Vector3 direction;

    private bool hitPlayer = false;

    void Start() {
        canFire = false;
    }

    public override void FixedUpdate() {

        // Raycast a theoretical bullet path to see if there are any obstacles in the way, if there are then don't shoot
        direction = parent.target - transform.position;
        //Debug.DrawRay(transform.position, direction, Color.cyan, 10);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100f, LayerMask.GetMask("Player", "Collisions/Ground", "Collisions/Obstacles"));

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) {
            Debug.DrawRay(transform.position, direction, Color.red, 10);
            hitPlayer = true;
        } else {
            hitPlayer = false;
        }

        // Firing cooldown timer
        if (!canFire) {
            timer += Time.deltaTime;
            if(timer > parent.attackCooldown) {
                canFire = true;
                timer = 0;
            }
        }

        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER
        && parent.enemyType != Enemy.EnemyType.DEAD) {

            // Firing logic, if not on cooldown and player in range, fire
            if (canFire && parent.inFollowRadius && hitPlayer && parent.seen) {
                parent.animator.SetBool("Attack", true);
                canFire = false;
                parent.attackCooldown = UnityEngine.Random.Range(parent.rangedAttackCooldownMin, parent.rangedAttackCooldownMax);
                instantBullet = Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
                StartCoroutine(BulletDestroy(2, instantBullet));
            }
            
        } else {
            this.gameObject.SetActive(false);
        }
    }
}
