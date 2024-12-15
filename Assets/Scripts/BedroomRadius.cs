using UnityEngine;

public enum RoomType {
    HOME, BEDROOM
}

public class BedroomRadius : MonoBehaviour
{
    public RoomType roomType;

    [SerializeField] private Animator animator;

    [SerializeField] private bool inRadius = false;

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.CompareTag("Player") && !inRadius) {
            inRadius = true;

            SwitchRooms();
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (collider.CompareTag("Player") && inRadius) {
            inRadius = false;
        }
    }

    private void SwitchRooms() {

        switch (roomType) {
            case RoomType.HOME:
                animator.SetBool("ToRoom", false);
                break;
            case RoomType.BEDROOM:
                animator.SetBool("ToRoom", true);
                break;
        }
    }
}
