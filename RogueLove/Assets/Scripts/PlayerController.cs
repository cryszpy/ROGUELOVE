using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [SerializeField]
    private ContactFilter2D movementFilter;

    [SerializeField]
    private PlayerAim aim;

    public CapsuleCollider2D contactColl;

    public Rigidbody2D rb;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Health Bar reference
    public HealthBar healthBar;

    // Energy Bar reference
    public EnergyBar energyBar;

    readonly List<RaycastHit2D> castCollisions = new();
    
    private VolumeProfile volumeProfile;

    [SerializeField]
    private CameraShake shake;

    [SerializeField] private CircleCollider2D dashColl;

    [Header("STATS")]

    public bool iFrame;
    
    private Vector2 movementInput;

    // Current direction vector
    [SerializeField] private Vector2 currentDirection;

    // Dash duration boolean
    [SerializeField] private bool isDashing = false;

    // Dash cooldown boolean
    [SerializeField] private bool canDash = true;

    // Dash duration
    [SerializeField] private float dashingTime;
    private float dashTimer = 0;

    // Dash cooldown time
    [SerializeField] private float dashingCooldown;
    private float dashcdTimer = 0;

    // Dash force / power
    [SerializeField] private float dashingPower;

    // Player max health
    public static float maxHealth;
    public static void SetMaxPlayerHealth(float hp) {
        maxHealth = hp;
    }
    public static float GetMaxPlayerHealth() {
        return maxHealth;
    }

    // Player current health
    public static float currentHealth;

    // Player movement speed
    private static float moveSpeed = 1.0f;
    public static void SetMoveSpeed(float speed) {
        moveSpeed = speed;
    }
    public static float GetMoveSpeed() {
        return moveSpeed;
    }

    // Player max energy
    public static float maxEnergy;
    public static void SetMaxEnergy(float num) {
        maxEnergy = num;
    }
    public static float GetMaxEnergy() {
        return maxEnergy;
    }

    public static float experience;
    public static void SetExperience(float exp) {
        experience = exp;
    }
    public static void AddExperience(float exp) {
        experience += exp;
    }
    public static float GetExperience() {
        return experience;
    }

    [SerializeField]
    private float collisionOffset = 0.01f;

    [SerializeField]
    private float shakeDuration;
    [SerializeField]
    private float shakeAmplitude;
    [SerializeField]
    private float shakeFrequency;

    public float Health {
        set {
            currentHealth = value;
            if(currentHealth <= 0) {
                DeathAnim();
                currentHealth = 0;
            }
        }

        get {
            return currentHealth;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) {
            rb = GetComponent<Rigidbody2D>();
            Debug.Log("PlayerController rb is null! Reassigned.");
        }
        if (animator == null) {
            animator = GetComponentInChildren<Animator>();
            Debug.Log("PlayerController animator is null! Reassigned.");
        }
        if (contactColl == null) {
            contactColl = GetComponentInChildren<CapsuleCollider2D>();
            Debug.Log("Collider2D contactColl is null! Reassigned.");
        }
        if (aim == null) {
            aim = GetComponentInChildren<PlayerAim>();
            Debug.Log("PlayerAim is null! Reassigned.");
        }
        if (volumeProfile == null) {
            volumeProfile = FindAnyObjectByType<Volume>().sharedProfile;
            Debug.Log("VolumeProfile volumeProfile is null! Reassigned.");
        }
        if (shake == null) {
            Debug.Log("CameraShake camShake is null! Reassigned.");
            shake = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraShake>();
        }

        // Set health bar and energy bar references on each stage load
        healthBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();
        energyBar = GameObject.FindGameObjectWithTag("EnergyBar").GetComponent<EnergyBar>();

        iFrame = false;

        string pathPlayer = Application.persistentDataPath + "/player.franny";

        // Load player info from saved game
        if (File.Exists(pathPlayer) && GameStateManager.SavePressed() == true) {
            GameStateManager.SetSave(false);

            LoadPlayer();
            //Debug.Log("Loaded player from save");
        } 
        // Save data exists but player did not click load save --> most likely a NextLevel() call
        else if (File.Exists(pathPlayer) && GameStateManager.SavePressed() == false) {
            GameStateManager.SetSave(false);
        } 
        // Save data does not exist, and player clicked load save somehow
        else if (!File.Exists(pathPlayer) && GameStateManager.SavePressed() == true) {
            Debug.LogError("Saved player data not found while trying to load save. How did you get here?");
        } 
        // Save data does not exist and player did not click load save --> most likely started new game
        else if (!File.Exists(pathPlayer) && GameStateManager.SavePressed() == false) {
            GameStateManager.SetSave(false);
            SetMaxPlayerHealth(6f);
            Health = maxHealth;
            SetMaxEnergy(20f);
            SetExperience(0f);
            SetMoveSpeed(1.0f);
        }

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(Health);
        energyBar.SetMaxEnergy(maxEnergy);
        energyBar.SetEnergy(0f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && canDash) {
            canDash = false;
            isDashing = true;
            animator.SetBool("Dash", true);
        }
    }
    
    private void FixedUpdate() {

        // Dash for the remaining duration, and don't take anything else as input
        if (isDashing) {

            // Enable reflection bullet radius
            dashColl.gameObject.SetActive(true);

            // Reset velocity to zero before dashing
            rb.velocity = Vector2.zero;
            TryDash(currentDirection);
            
            // Dash duration timer
            dashTimer += Time.fixedDeltaTime;
                
            if(dashTimer > dashingTime) {

                // Disable reflection bullet radius
                dashColl.gameObject.SetActive(false);

                // End dash
                isDashing = false;

                // End dash animation
                animator.SetBool("Dash", false);

                // Reser velocity to zero after dashing
                rb.velocity = Vector2.zero;

                // Reset dash duration timer
                dashTimer = 0;
            }

            return;
        }

        // Movement system if you're not dead lol
        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER) {

            // Dash cooldown timer
            if (!isDashing && !canDash) {
                dashcdTimer += Time.fixedDeltaTime;

                if(dashcdTimer > dashingCooldown) {

                    // Reset dash cooldown
                    canDash = true;

                    // Reset dash cooldown timer
                    dashcdTimer = 0;
                }
            }

            if (movementInput != Vector2.zero) {
                bool success = TryMove(movementInput);
                currentDirection = movementInput;

                if (!success) {
                    success = TryMove(new Vector2(movementInput.x, 0));
                    currentDirection = new Vector2(movementInput.x, 0);

                    if (!success) {
                        success = TryMove(new Vector2(0, movementInput.y));
                        currentDirection = new Vector2(0, movementInput.y);
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
        }
    }

    // Dash function
    private bool TryDash(Vector2 direction) {

        if (direction != Vector2.zero) {

            int count = rb.Cast(
                direction, 
                movementFilter, 
                castCollisions, 
                moveSpeed * Time.fixedDeltaTime + collisionOffset);
        
            if(count == 0) {
                rb.AddForce(direction * dashingPower * Time.fixedDeltaTime, ForceMode2D.Impulse);
                return true;
            } else {
                return false;
            }
        } else {
            // can't move if there's no direction to move in
            return false;
        }
    }

    // Move function
    private bool TryMove(Vector2 direction) {
        if (direction != Vector2.zero) {
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
        /*
        animator.SetTrigger("MeleeAttack");
        */
    }

    public void TakeDamage(float damage) {

        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER && iFrame == false) {
            iFrame = true;
            StartCoroutine(SetHurtFlash(true));
            StartCoroutine(shake.Shake(shakeDuration, shakeAmplitude, shakeFrequency));
            Health -= damage;
            healthBar.SetHealth(currentHealth);
            Debug.Log("Player took damage!");

            animator.SetBool("Hurt", true);
        }
    }

    private IEnumerator SetHurtFlash(bool condition) {

        if (volumeProfile.TryGet<ColorAdjustments>(out var colorAdjust)) {
            colorAdjust.active = condition;
            yield return new WaitForSeconds(0.2f);
            colorAdjust.active = !condition;
        }

        yield return null;
    }

    public void SavePlayer () {
        SaveSystem.SavePlayer(this, aim);
    }

    public void LoadPlayer() {
        
        // Load save data
        PlayerData data = SaveSystem.LoadPlayer();

        SetMaxPlayerHealth(data.playerMaxHealth);
        healthBar.SetMaxHealth(maxHealth);

        // Set health
        Health = data.playerHealth;
        healthBar.SetHealth(Health);

        // Load experience level
        SetMaxEnergy(data.maxExperienceLevel);
        SetExperience(data.experienceLevel);
        energyBar.SetMaxEnergy(data.maxExperienceLevel);
        energyBar.SetEnergy(data.experienceLevel);

        // Set speeds
        SetMoveSpeed(data.playerMoveSpeed);
        aim.timeBetweenFiring = data.playerAttackSpeed;
    }

    public void DeathAnim() {
        animator.SetBool("Death", true);
        FindFirstObjectByType<AudioManager>().Play("PlayerDeath");
    }
}
