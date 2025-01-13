using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField]
    private Collider2D coll;

    [SerializeField]
    private Animator animator;

    private void OnTriggerEnter2D(Collider2D collider) {

        // If collided with the player or a bullet, break apart
        if (collider.CompareTag("Player") || collider.CompareTag("Projectile")) {
            coll.enabled = false;
            animator.SetTrigger("Break");
        } 
    }
}
