using System.Collections;
using System.Collections.Generic;
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
        parent.rb.velocity = Vector3.zero;
        parent.rb.AddForce(direction.normalized * knockback, ForceMode2D.Impulse);
        StartCoroutine(EnemyKnockback(parent.rb));
        //Debug.Log("ADDED FORCE");
    }

    IEnumerator EnemyKnockback(Rigidbody2D rigidBody) {
        parent.kbEd = true;
        yield return new WaitForSeconds(0.15f);
        rigidBody.velocity = Vector3.zero;
        parent.kbEd = false;
    }

    public void Death() {
        animator.SetTrigger("Death");
    }
}
