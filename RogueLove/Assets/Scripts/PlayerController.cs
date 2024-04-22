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

    public Collider2D contactColl;

    public bool iFrame;
    
    private Vector2 movementInput;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Health Bar reference
    public HealthBar healthBar;

    readonly List<RaycastHit2D> castCollisions = new();
    
    private VolumeProfile volumeProfile;

    [Space(10)]
    [Header("STATS")]

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

    [SerializeField]
    private float collisionOffset = 0.01f;

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
            contactColl = GetComponentInChildren<Collider2D>();
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

        healthBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();
        iFrame = false;

        string pathPlayer = Application.persistentDataPath + "/player.franny";
        //Debug.Log(File.Exists(pathPlayer));
        //Debug.Log(GameStateManager.SavePressed());

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
            Debug.LogError("Save data not found while trying to load save. How did you get here?");
        } 
        // Save data does not exist and player did not click load save --> most likely started new game
        else if (!File.Exists(pathPlayer) && GameStateManager.SavePressed() == false) {
            GameStateManager.SetSave(false);
            SetMaxPlayerHealth(20f);
            Health = maxHealth;
            SetMoveSpeed(1.0f);
        }

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(Health);
        //Debug.Log("MAX HEALTH " + maxHealth);
        //Debug.Log("HEALTH " + Health);
    }
    
    private void Update() {

        // Movement system if you're not dead lol
        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER) {
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
        /*
        animator.SetTrigger("MeleeAttack");
        */
    }

    public void TakeDamage(float damage) {

        if (GameStateManager.GetState() != GameStateManager.GAMESTATE.GAMEOVER && iFrame == false) {
            iFrame = true;
            StartCoroutine(SetHurtFlash(true));
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
        //Debug.Log("LOAD PLAYER MAX HEALTH: " + data.playerMaxHealth);
        healthBar.SetMaxHealth(maxHealth);

        // Set health
        Health = data.playerHealth;
        //Debug.Log("LOAD PLAYER CURRENT HEALTH: " + data.playerHealth);
        healthBar.SetHealth(Health);

        // Set speeds
        SetMoveSpeed(data.playerMoveSpeed);
        aim.timeBetweenFiring = data.playerAttackSpeed;
    }

    public void DeathAnim() {
        animator.SetBool("Death", true);
        FindFirstObjectByType<AudioManager>().Play("PlayerDeath");
    }
}
