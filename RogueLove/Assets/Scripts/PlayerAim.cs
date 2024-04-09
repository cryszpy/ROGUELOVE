using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;

    public GameObject bullet;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    private float destroyTimer;
    public float timeBetweenFiring;

    private GameObject instantBullet;

    [SerializeField]
    private SpriteRenderer gun;

    void Start() {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void FixedUpdate() {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (gun == null) {
            Debug.Log("PlayerAim gun is null! Reassigned.");
            gun = GetComponentInChildren<SpriteRenderer>();
        }

        if (!canFire) {
            timer += Time.deltaTime;
            if(timer > timeBetweenFiring) {
                canFire = true;
                timer = 0;
            }
        }

        if (GameStateManager.getState() != GameStateManager.GAMESTATE.GAMEOVER) {

            // Flips weapon sprite depending on mouse orientation to character
            if (this.gameObject.transform.rotation.z > 0.7f || this.gameObject.transform.rotation.z < -0.7f) {
                gun.flipY = true;
                //Debug.Log("flipped");
            } else {
                gun.flipY = false;
            }

            // Firing logic, if not on cooldown and mouse button pressed, fire
            if (Input.GetMouseButton(0) && canFire) {
                canFire = false;
                instantBullet = (GameObject) Instantiate(bullet, bulletTransform.position, Quaternion.identity);
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
