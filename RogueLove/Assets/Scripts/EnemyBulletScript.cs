using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    private Vector3 target;

    public Rigidbody2D rb;

    [SerializeField] private Animator animator;

    private Vector3 direction;

    [SerializeField] private Collider2D coll;

    [Header("STATS")]

    [SerializeField] private float force;

    [SerializeField] private float damage = 2;

    [SerializeField] private bool reflected = false;

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

        // Checks whether collided object is of layer Player
        if (other.gameObject.layer == 3) {

            // If not the reflect dash radius
            if (!other.CompareTag("PlayerDashRadius")) {

                // Deal damage to player if not null
                if (other != null) {
                                
                    if (other.TryGetComponent<PlayerController>(out var player)) {
                        player.TakeDamage(damage);
                    }

                    // Destroy bullet on contact with player
                    coll.enabled = false;
                    rb.velocity = (Vector2)direction.normalized * 0;
                    animator.SetTrigger("Destroy");
                }
                else {
                    Debug.LogWarning("Enemy bullet collision is null!");
                }
            }
            // If collided object is the dash radius
            else {

                // Reflect bullet
                Vector2 temp = rb.velocity;
                rb.velocity = Vector2.zero;
                rb.velocity = new Vector2(temp.x * -1, temp.y * -1);
                reflected = true;
            }
            
        }
        // If hitting anything other than the player (should destroy) or an enemy (should passthrough)
        // then destroy bullet
        else if (!other.CompareTag("Enemy")) {
            coll.enabled = false;
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
        
        if (other.CompareTag("Enemy") && reflected) {
            if (other.TryGetComponent<EnemyHealth>(out var enemy)) {
                enemy.TakeDamage(damage, rb.velocity);
            }

            // Destroy bullet on contact with enemy after reflection
            coll.enabled = false;
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }

    public void DestroyBullet() {
        Destroy(gameObject);
    }
}
