using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public Vector3 target;

    [SerializeField]
    private Rigidbody2D rb;

    public Animator animator;

    public Vector3 direction;

    [SerializeField]
    private Collider2D coll;

    [Space(10)]
    [Header("STATS")]

    [SerializeField]
    private float force;

    [SerializeField]
    private float damage = 2;

    // Start is called before the first frame update
    void Start()
    {

        coll.enabled = true;

        if (animator == null) {
            Debug.Log("BulletScript animator is null! Reassigned.");
            animator = GetComponent<Animator>();
        }

        if (rb == null) {
            Debug.Log("BulletScript rb is null! Reassigned.");
            rb = GetComponent<Rigidbody2D>();
        }

        target = GameObject.FindGameObjectWithTag("Player").transform.position;
        direction = target - transform.position;
        Vector3 rotation =  transform.position - target;

        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    // Damage player or destroy self when hitting obstacles
    private void OnTriggerEnter2D(Collider2D other) {
        
        direction = target - transform.position;

        // Checks whether collided object is an enemy
        if (other.CompareTag("Player")) {
            // Deal damage to enemy
            if (other != null) {
                            
                if (other.TryGetComponent<PlayerController>(out var player)) {
                    player.TakeDamage(damage);
                }
            }
        }

        if (!other.CompareTag("Enemy")) {
            coll.enabled = false;
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }

    public void DestroyBullet() {
        Destroy(gameObject);
    }
}
