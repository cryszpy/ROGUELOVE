using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public SwordAttack swordAttack;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    void Start() {
        if (spriteRenderer == null) {
            Debug.Log("PlayerAnimations spriteRenderer is null! Reassigned.");
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    
    public void SwordAttack() {
        if(spriteRenderer.flipX == true) {
            swordAttack.AttackLeft();
        } else {
            swordAttack.AttackRight();
        }
    }

    public void EndSwordAttack() {
        swordAttack.StopAttack();
    }
}
