using UnityEngine;

public class EnemyWeaponRaycast : MonoBehaviour
{

    [SerializeField] private Enemy enemy;

    public virtual void FixedUpdate() {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING && enemy.enemyType != EnemyType.DEAD && !enemy.kbEd) {

            // Raycast a theoretical bullet path to see if there are any obstacles in the way, if there are then don't shoot
            Vector3 direction = enemy.player.position - transform.position;
            Debug.DrawRay(transform.position, direction, Color.cyan, 0.05f);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100f, LayerMask.GetMask("Player", "Collisions/Ground", "Collisions/Obstacles"));

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) {
                Debug.DrawRay(transform.position, direction, Color.red, 0.05f);
                enemy.hitPlayer = true;
                enemy.seen = true;
                
            } else {
                enemy.hitPlayer = false;
            }
        }
    }
}
