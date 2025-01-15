using UnityEngine;

public class EnemyWeaponRaycast : MonoBehaviour
{

    [SerializeField] private Enemy enemy;

    [SerializeField] private LayerMask layerMask;

    public virtual void FixedUpdate() {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING && enemy.enemyType != EnemyType.DEAD && !enemy.kbEd) {

            // Raycast a theoretical bullet path to see if there are any obstacles in the way, if there are then don't shoot
            Vector3 direction = enemy.player.position - enemy.transform.position;
            Debug.DrawRay(enemy.transform.position, direction, Color.yellow, 0.05f);

            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction, 50f, layerMask);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player")) {
                Debug.DrawRay(enemy.transform.position, direction, Color.red, 0.05f);
                enemy.hitPlayer = true;
                enemy.seen = true;
                
            } else {
                enemy.hitPlayer = false;
            }
        }
    }
}
