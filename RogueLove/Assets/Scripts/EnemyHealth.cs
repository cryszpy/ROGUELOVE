using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // Animator component
    public Animator animator;

    // Health bar
    public HealthBar healthBar;

    // Enemy current health
    public float currentHealth;

    // Enemy maximum health
    public float maxHealth;

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
        healthBar = this.GetComponentInChildren<HealthBar>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        if (animator == null) {
            animator = this.gameObject.GetComponent<Animator>();
        }
    }

    public void HurtCheck() {
        animator.SetBool("Hurt", false);
    }

    public void TakeDamage(float damage) {
        Health -= damage;
        healthBar.SetHealth(currentHealth);

        animator.SetBool("Hurt", true);

    }

    public void Death() {
        animator.SetTrigger("Death");
    }
}
