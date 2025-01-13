using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public SwordAttack swordAttack;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private PlayerController parent;

    [SerializeField]
    private Animator animator;

    void Start() {
        if (spriteRenderer == null) {
            Debug.Log("PlayerAnimations spriteRenderer is null! Reassigned.");
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (animator == null) {
            Debug.Log("PlayerController animator is null! Reassigned.");
            animator = GetComponent<Animator>();
        }
    }
    
    public void SwordAttack() {
        /*
        if(spriteRenderer.flipX == true) {
            swordAttack.AttackLeft();
        } else {
            swordAttack.AttackRight();
        }
        */
    }

    public void EndSwordAttack() {
        //swordAttack.StopAttack();
    }

    public void HurtCheck() {
        animator.SetBool("Hurt", false);
        parent.iFrame = false;

    }

    public void PlayerDeath() {
        animator.SetBool("Death", false);
        GameStateManager.GameOver();
    }
}
