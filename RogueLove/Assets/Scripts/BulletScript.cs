using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    protected Vector3 mousePos;
    protected Camera mainCam;

    [SerializeField]
    protected Rigidbody2D rb;

    public Animator animator;

    protected Vector3 direction;

    [SerializeField]
    protected Collider2D coll;

    [SerializeField]
    protected Weapon weapon;

    [Space(10)]
    [Header("STATS")]

    [SerializeField]
    protected float force;

    public float damage;

    [Tooltip("Lower values are more accurate— 0 fires in a straight line.")]
    [SerializeField]
    protected float accuracy;

    [SerializeField] protected bool isFire;

    // Start is called before the first frame update
    public virtual void Start()
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

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // DIRECTION OF THE BULLET

        direction = mousePos - weapon.transform.position;

        if (direction == Vector3.zero) {
            direction = weapon.spawnPos.transform.position - weapon.transform.position;
        }
       
        Vector3 rotation =  transform.position - mousePos;

        Vector2 error = UnityEngine.Random.insideUnitCircle * accuracy;

        rb.velocity = new Vector2(direction.x, direction.y).normalized * force + new Vector2(error.x, error.y);
        // Rotation of the bullet (which way it is facing, NOT which direction its moving in)
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    public UnityEngine.Object Create(UnityEngine.Object original, Vector3 position, Quaternion rotation, Weapon weapon, Camera cam) {
        GameObject bullet = Instantiate(original, position, rotation) as GameObject;
        
        if (bullet.TryGetComponent<BulletScript>(out var script)) {
            script.weapon = weapon;
            script.mainCam = cam;
            return bullet;
        } else if (bullet.GetComponentInChildren<BulletScript>()) {
            BulletScript bulletScript = bullet.GetComponentInChildren<BulletScript>();
            bulletScript.weapon = weapon;
            bulletScript.mainCam = cam;
            return bullet;
        } else {
            Debug.LogError("Could not find BulletScript script or extension of such on this Object.");
            return null;
        }
    }

    // Damage enemies or destroy self when hitting obstacles
    public virtual void OnTriggerEnter2D(Collider2D other) {
        
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

        // If collided object layer is not player (3), weapon (12), or breakables (10)
        if (other.gameObject.layer != 3  && other.gameObject.layer != 12 && other.gameObject.layer != 10 && other.gameObject.tag != "Projectile") {
            coll.enabled = false;
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }

    public virtual void DestroyBullet() {
        Destroy(gameObject);
    }
}
