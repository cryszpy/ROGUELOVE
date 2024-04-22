using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public Camera mainCam;
    [SerializeField]
    private CameraShake shake;
    public Vector3 mousePos;
    public GameObject bullet;
    public Transform bulletSpawnPos;
    public GameObject instantBullet;
    public SpriteRenderer gun;

    [Space(10)]
    [Header("STATS")]

    public bool canFire;
    public float timer;
    public float timeBetweenFiring;

    [SerializeField]
    private float shakeDuration;
    [SerializeField]
    private float shakeAmplitude;
    [SerializeField]
    private float shakeFrequency;

    void Start() {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public virtual void FixedUpdate() {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (gun == null) {
            Debug.Log("PlayerAim gun is null! Reassigned.");
            gun = GetComponentInChildren<SpriteRenderer>();
        }
        if (shake == null) {
            Debug.Log("CameraShake camShake is null! Reassigned.");
            shake = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraShake>();
        }

        if (!canFire) {
            timer += Time.deltaTime;
            if(timer > timeBetweenFiring) {
                canFire = true;
                timer = 0;
            }
        }

        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER) {

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
                FindFirstObjectByType<AudioManager>().Play("PlayerBullet");
                instantBullet = Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
                StartCoroutine(shake.Shake(shakeDuration, shakeAmplitude, shakeFrequency));
                //camShake.Shake(0.15f, 0.4f);
                StartCoroutine(BulletDestroy(2, instantBullet));
            }
        } else {
            this.gameObject.SetActive(false);
        }
        
    }

    // Destroy bullet if it doesn't hit an obstacle and keeps traveling after some time
    public IEnumerator BulletDestroy(float waitTime, GameObject obj) {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            DestroyImmediate(obj, true);
        }
    }
}
