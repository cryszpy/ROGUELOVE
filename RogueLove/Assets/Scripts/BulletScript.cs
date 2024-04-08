using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    public Rigidbody2D rb;
    public float force;

    public Animator animator;

    Vector3 direction;

    public Collider2D bulletCollider;

    public float damage = 3;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePos - transform.position;
        Vector3 rotation =  transform.position - mousePos;

        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    // Damage enemies or destroy self when hitting obstacles
    private void OnTriggerEnter2D(Collider2D other) {
        
        direction = mousePos - transform.position;

        // Checks whether collided object is an enemy
        if (other.CompareTag("Enemy")) {
            // Deal damage to enemy
            if (other != null) {
                            
                if (other.TryGetComponent<EnemyHealth>(out var enemy)) {
                    enemy.TakeDamage(damage);
                }
            }
            
        }

        if (other.gameObject.layer != 3) {
            //Debug.Log(other.gameObject.layer);
            rb.velocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }

    public void DestroyBullet() {
        Destroy(gameObject);
    }
}
