using System.Collections;
using UnityEngine;

public class EnemyOrbitalBullet : MonoBehaviour
{
    [Header("SCRIPT REFERENCES")]

    public Enemy parent;

    [SerializeField] protected Animator animator;

    [Header("STATS")]

    public int attackDamage = 1;

    public float attackTime;
    protected float attackTimer = 0;

    [Tooltip("The time that the collider lingers for after the attack if it hasn't hit the player. (seconds)")]
    public float lingerTime;

    [Tooltip("Whether the player is inside this attack radius.")]
    protected bool inRadius = false;

    [Tooltip("Whether this attack has already hit the player or not.")]
    protected bool hasHitPlayer = false;

    [Tooltip("Whether the attack is currently active or not.")]
    protected bool attackIsActive = false;

    [Tooltip("Whether the attack destruction sequence has started or not.")]
    protected bool destroying = false;

    public virtual void Update() {

        // Starts the wait time before attacking
        if (!attackIsActive && !destroying) {
            PrepareAttack();
        }

        // If the player is in radius, and the attack is active, and has not hit the player yet—
        if (inRadius && attackIsActive && !hasHitPlayer) {

            // Stops infinite looping of this function
            hasHitPlayer = true;

            // Damage the player
            parent.player.TakeDamage(attackDamage);
        }
    }

    // Timer that waits until the allotted time to attack
    public virtual void PrepareAttack() {

        attackTimer += Time.deltaTime;

        if (attackTimer > attackTime) {
            attackTimer = 0;

            // Trigger attack animation to begin
            animator.SetTrigger("Attack");
        }
    }

    public virtual void OnTriggerEnter2D (Collider2D collider) {

        if (collider.CompareTag("Player") && parent.enemyType != EnemyType.DEAD) {

            // Set to player inside radius
            inRadius = true;

        } 
    }

    public virtual void OnTriggerExit2D (Collider2D collider) {

        if (collider.CompareTag("Player")) {

            // Reset player inside radius
            inRadius = false;

        } 
    }

    // CALLED AFTER ATTACK ANIMATION IS PLAYED (start damaging)
    public virtual void OnAttackAnimEnd() {

        // Attack can now damage players
        attackIsActive = true;

        StartCoroutine(AttackLinger());
    }

    public virtual IEnumerator AttackLinger() {

        // Waits for the specified linger time
        yield return new WaitForSeconds(lingerTime);

        // Deactivates attack
        destroying = true;
        attackIsActive = false;
    }

    // CALLED AFTER ATTACK END ANIMATION IS PLAYED
    public virtual void DestroyAttack() {
        Destroy(gameObject);
    }

    public virtual UnityEngine.Object Create(UnityEngine.Object original, Vector3 position, Enemy enemy, Quaternion rotation = default) {

        if (rotation == default) {
            rotation = Quaternion.identity;
        }

        GameObject bullet = Instantiate(original, position, rotation) as GameObject;
        
        if (bullet.TryGetComponent<EnemyOrbitalBullet>(out var script)) {
            script.parent = enemy;
            return bullet;
        } else if (bullet.GetComponentInChildren<EnemyOrbitalBullet>()) {
            bullet.GetComponentInChildren<EnemyOrbitalBullet>().parent = enemy;
            return bullet;
        } else {
            Debug.LogError("Could not find EnemyOrbitalAttack script or extension of such on this Object.");
            return null;
        }
    }
}
