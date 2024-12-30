using UnityEngine;

public class NPCDirectionFacing : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Animator animator;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        DirectionFacing();
    }

    private void DirectionFacing() {

        // Face direction of movement
        if (rb.linearVelocity.x >= 0.001f) {

            this.transform.localScale = new Vector3(1f, 1f, 1f);
            animator.SetBool("IsMoving", true);

        } else if (rb.linearVelocity.x <= -0.001f) {

            this.transform.localScale = new Vector3(-1f, 1f, 1f);
            animator.SetBool("IsMoving", true);

        } else if (rb.linearVelocity.y <= -0.001 || rb.linearVelocity.y >= 0.001) {
            animator.SetBool("IsMoving", true);
        }
        // Standing still, face player
        else {
            animator.SetBool("IsMoving", false);

            if (player.transform.position.x > transform.position.x) {
                this.transform.localScale = new Vector3(1f, 1f, 1f);
            } else if (player.transform.position.x < transform.position.x) {
                this.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }
}
