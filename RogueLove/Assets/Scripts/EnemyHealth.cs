using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    // Animator component
    public Animator animator;

    [SerializeField]
    private Enemy parent;

    // Health bar
    public HealthBar healthBar;

    [Space(10)]
    [Header("STATS")]

    // Enemy current health
    public float currentHealth;

    // Enemy maximum health
    public float maxHealth;

    [SerializeField]
    private float knockback;

    public float Health {
        set {
            currentHealth = value;
            if(currentHealth <= 0) {
                Death();
            }
        }

        get {
            return currentHealth;
        }
    }

    void Start() {
        if (healthBar == null) {
            Debug.Log("EnemyHealth healthbar is null! Reassigned.");
            healthBar = this.GetComponentInChildren<HealthBar>();
        }
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        if (animator == null) {
            Debug.Log("EnemyHealth animator is null! Reassigned.");
            animator = this.gameObject.GetComponent<Animator>();
        }
    }

    public void HurtCheck() {
        animator.SetBool("Hurt", false);
    }

    public void TakeDamage(float damage, Vector3 direction) {
        Health -= damage;
        Debug.Log("Took this amount of damage: " + damage);
        healthBar.SetHealth(currentHealth);
        animator.SetBool("Hurt", true);
        parent.kbEd = true;
        StopAllCoroutines();
        if (Health <= 0) {
            StartCoroutine(EnemyDeathKnockback(parent.rb, direction));
        } else {
            StartCoroutine(EnemyKnockback(parent.rb, direction));
        }
    }

    IEnumerator EnemyKnockback(Rigidbody2D rigidBody, Vector3 dir) {
        parent.kbEd = true;
        parent.rb.velocity = Vector3.zero;
        parent.rb.AddForce(dir.normalized * knockback, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        rigidBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.35f);
        parent.kbEd = false;
    }

    IEnumerator EnemyDeathKnockback(Rigidbody2D rigidBody, Vector3 dir) {
        parent.kbEd = true;
        parent.rb.velocity = Vector3.zero;
        parent.rb.AddForce(dir.normalized * (knockback * 3), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        rigidBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.35f);
        parent.kbEd = false;
    }

    public void Death() {
        parent.hitbox.enabled = false;
        animator.SetTrigger("Death");
    }
}
