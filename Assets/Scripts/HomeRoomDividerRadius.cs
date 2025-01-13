using UnityEngine;

public class HomeRoomDividerRadius : MonoBehaviour
{
    public bool triggered;
    
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            triggered = false;
        }
    }
}
