using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    // Animator component
    public Animator animator;

    [SerializeField] private Enemy parent;

    [Space(10)]
    [Header("STATS")]

    // Enemy current health
    public float currentHealth;

    // Enemy maximum health
    public float maxHealth;

    [SerializeField] private float knockback;

    public bool takingFireDamage;

    public bool immuneToFire;

    public float Health {
        set {
            currentHealth = value;
            if(currentHealth <= 0) {
                StartDeath();
            }
        }

        get {
            return currentHealth;
        }
    }

    void Start() {

        currentHealth = maxHealth;

        if (animator == null) {
            Debug.Log("EnemyHealth animator is null! Reassigned.");
            animator = this.gameObject.GetComponent<Animator>();
        }
    }

    public void HurtCheck() {
        animator.SetBool("Hurt", false);
    }

    public void TakeDamage(float damage, Vector2 direction) {
        Health -= damage;
        //Debug.Log("Took this amount of damage: " + damage);

        animator.SetBool("Hurt", true);
        parent.kbEd = true;
        if (Health <= 0) {
            StopAllCoroutines();
            StartCoroutine(EnemyDeathKnockback(parent.rb, direction.normalized));
        } else {
            StopAllCoroutines();
            StartCoroutine(EnemyKnockback(parent.rb, direction.normalized));
        }
    }

    public void TakeFireDamage(float damage, Vector2 direction) {
        Health -= damage;
        //Debug.Log("Took this amount of damage: " + damage);

        animator.SetBool("Hurt", true);
        parent.kbEd = true;
        if (Health <= 0) {
            StopAllCoroutines();
            StartCoroutine(EnemyDeathKnockback(parent.rb, direction.normalized));
        } else {
            StopAllCoroutines();
            StartCoroutine(EnemyKnockback(parent.rb, direction.normalized));
        }
        animator.SetBool("FireHurt", true);
        StartCoroutine(FireDamage(2));
    }

    public IEnumerator FireDamage(float damage) {
        takingFireDamage = true;
        Health -= damage;
        //Debug.Log("Took this amount of FIRE damage: " + damage);

        yield return new WaitForSeconds(1f);

        Health -= damage;
        //Debug.Log("Took this amount of FIRE damage: " + damage);

        yield return new WaitForSeconds(1f);

        Health -= damage;
        //Debug.Log("Took this amount of FIRE damage: " + damage);

        takingFireDamage = false;
    }

    public void FireDamageAnim() {
        if (takingFireDamage) {
            animator.SetBool("FireHurt", true);
        }
        else {
            animator.SetBool("FireHurt", false);
        }
    }

    IEnumerator EnemyKnockback(Rigidbody2D rigidBody, Vector2 dir) {
        parent.kbEd = true;
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(dir * knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.15f);

        rigidBody.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.35f);

        parent.kbEd = false;
    }

    IEnumerator EnemyDeathKnockback(Rigidbody2D rigidBody, Vector2 dir) {
        parent.kbEd = true;
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(dir * (knockback * 2), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        rigidBody.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.35f);

        parent.kbEd = false;
    }

    public void StartDeath() {
        animator.SetTrigger("Death");
    }
}
