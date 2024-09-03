using UnityEngine;

public class BasePickup : MonoBehaviour
{
    public GameObject objectToSpawn;

    protected PlayerController player;
    
    protected bool playerFound;

    protected bool playerInRadius;

    public bool dropped = false;

    public virtual void Awake() {
        playerFound = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {

            playerInRadius = true;

            if (collider.gameObject.TryGetComponent<PlayerController>(out var controller)) {
                player = controller;
                playerFound = true;
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            playerInRadius = false;
        }
    }

    public virtual void Update() {

        if (!playerFound) {
            return;
        }
    }

    public virtual void RemoveObject() {
        Destroy(gameObject);
    }
}
