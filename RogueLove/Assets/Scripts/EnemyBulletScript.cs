using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    protected Vector3 target;

    public Rigidbody2D rb;

    [SerializeField] protected Animator animator;

    public Vector3 direction;

    [SerializeField] protected Collider2D coll;

    [Header("STATS")]

    [SerializeField] protected Vector2 spawnPoint;

    [SerializeField] protected float speed;

    [SerializeField] protected int damage = 2;

    [SerializeField] protected float knockback;

    [SerializeField] protected bool reflected = false;

    protected float timer = 0f;

    protected Vector2 error;

    [Tooltip("Lower values are more accurate— 0 fires in a straight line.")]
    [SerializeField] protected float accuracy;

    // Start is called before the first frame update
    public virtual void Start()
    {

        spawnPoint = new Vector2(transform.position.x, transform.position.y);

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

        //direction = target - transform.position;

        Vector3 rotation = transform.position - target;

        // Determines the accuracy of the bullet (so bullets don't just fire in a straight line every time)
        error = UnityEngine.Random.insideUnitCircle * accuracy;

        // Sets the velocity and direction of the bullet which is acted on every frame from now on (this determines how the bullet moves)
        rb.velocity = new Vector2(transform.right.x, transform.right.y) * speed + new Vector2(error.x, error.y);

        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    /* public virtual void Update() {
        timer += Time.deltaTime;
        //transform.position = Movement(timer);
    }

    public virtual Vector2 Movement(float timer) {
        float x = timer * speed * transform.up.x;
        float y = timer * speed * transform.up.y;
        return new Vector2(x + spawnPoint.x + error.x, y + spawnPoint.y + error.y);
    } */

    // Damage player or destroy self when hitting obstacles
    public virtual void OnTriggerEnter2D(Collider2D other) {
        
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
                enemy.TakeDamage(damage, rb.velocity, knockback);
            }

            // Destroy bullet on contact with enemy after reflection
            coll.enabled = false;
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }

    public virtual void DestroyBullet() {
        Destroy(gameObject);
    }
}
