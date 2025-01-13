using UnityEngine;

public class EnemyFollowRadius : MonoBehaviour
{
    [SerializeField] protected Enemy parent;

    public virtual void OnTriggerEnter2D(Collider2D collider) {

        if (collider.CompareTag("Player")) {
            parent.inFollowRadius = true;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collider) {
        if(collider.CompareTag("Player")) {
            parent.inFollowRadius = false;
        }
    }
}
