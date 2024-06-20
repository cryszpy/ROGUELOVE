using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    private Vector3 mousePos;
    private Camera mainCam;

    [SerializeField]
    private Rigidbody2D rb;

    public Animator animator;

    private Vector3 direction;

    [SerializeField]
    private Collider2D coll;

    [Space(10)]
    [Header("STATS")]

    [SerializeField]
    private float force;

    public float damage;

    [SerializeField]
    private float accuracy;

    [SerializeField] private bool isFire;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        coll.enabled = true;

        if (animator == null) {
            Debug.Log("BulletScript animator is null! Reassigned.");
            animator = GetComponent<Animator>();
        }
        if (rb == null) {
            Debug.Log("BulletScript rb is null! Reassigned.");
            rb = GetComponent<Rigidbody2D>();
        }

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        // Direction of the bullet
        direction = mousePos - transform.position;
       
        Vector3 rotation =  transform.position - mousePos;

        Vector2 error = UnityEngine.Random.insideUnitCircle * accuracy;

        rb.velocity = new Vector2(direction.x, direction.y).normalized * force + new Vector2(error.x, error.y);
        // Rotation of the bullet (which way it is facing, NOT which direction its moving in)
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    // Damage enemies or destroy self when hitting obstacles
    private void OnTriggerEnter2D(Collider2D other) {
        
        direction = mousePos - transform.position;

        // Checks whether collided object is an enemy
        if (other.CompareTag("Enemy")) {

            // Tries to get the EnemyHealth component of collided enemy
            if (other.TryGetComponent<EnemyHealth>(out var enemy)) {

                // If bullet is a flame bullet, deal fire damage
                if (isFire && !enemy.immuneToFire) {
                    enemy.TakeFireDamage(damage, direction);
                }
                else {
                    enemy.TakeDamage(damage, direction);
                }
            }
            else {
                Debug.LogError("Could not get collided enemy's EnemyHealth component!");
            }
        }

        if (other.gameObject.layer != 3 && other.gameObject.layer != 10) {
            coll.enabled = false;
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }

    public void DestroyBullet() {
        Destroy(gameObject);
    }
}
