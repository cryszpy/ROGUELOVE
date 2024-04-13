using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletSpawnPos;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;

    private GameObject instantBullet;

    [SerializeField]
    private Enemy parent;

    private Vector3 direction;

    private bool hitPlayer = false;

    void FixedUpdate() {

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
            if(timer > timeBetweenFiring) {
                canFire = true;
                timer = 0;
            }
        }

        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER) {

            // Firing logic, if not on cooldown and player in range, fire
            if (canFire && parent.inFollowRadius && hitPlayer) {
                canFire = false;
                timeBetweenFiring = UnityEngine.Random.Range(2, 4);
                instantBullet = Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
                StartCoroutine(BulletDestroy(2, instantBullet));
            }
            
        } else {
            this.gameObject.SetActive(false);
        }
    }

    // Destroy bullet if it doesn't hit an obstacle and keeps traveling after some time
    private IEnumerator BulletDestroy(float waitTime, GameObject obj) {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            DestroyImmediate(obj, true);
        }
    }
}
