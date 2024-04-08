using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 1.5f;

    [SerializeField]
    private float collisionOffset = 0.01f;

    [SerializeField]
    private ContactFilter2D movementFilter;
    
    private Vector2 movementInput;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    readonly List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Player max health
    public float maxHealth = 100;

    // Player current health
    public float currentHealth;

    // Health Bar reference
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) {
            Debug.Log("PlayerController rb is null! Reassigned.");
            rb = GetComponent<Rigidbody2D>();
        }
        if (animator == null) {
            Debug.Log("PlayerController animator is null! Reassigned.");
            animator = GetComponentInChildren<Animator>();
        }
        //spriteRenderer = GetComponent<SpriteRenderer>();

        healthBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    
    private void Update() {
        if(movementInput != Vector2.zero){
            bool success = TryMove(movementInput);

            if (!success) {
                success = TryMove(new Vector2(movementInput.x, 0));

                if (!success) {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }

            animator.SetBool("IsMoving", success);
        } else {
            animator.SetBool("IsMoving", false);
        }

        // Set direction of sprite to movement direction
        if(movementInput.x < 0) {
            spriteRenderer.flipX = true;
        } else if (movementInput.x > 0) {
            spriteRenderer.flipX = false;
        }

        // Test health
        if(Input.GetKeyDown(KeyCode.Space)) {
            TakeDamage(20);
        }

    }

    private bool TryMove(Vector2 direction) {
        if(direction != Vector2.zero) {
            int count = rb.Cast(
                direction, 
                movementFilter, 
                castCollisions, 
                moveSpeed * Time.fixedDeltaTime + collisionOffset);
        
            if(count == 0) {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
        } else {
            // can't move if there's no direction to move in
            return false;
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("MeleeAttack");
    }

    void TakeDamage(float damage) {

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

    }
}
