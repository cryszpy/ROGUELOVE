using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;

public class ContactEnemy : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    // This enemy's Animator component
    public Animator animator;

    // This enemy's target
    [SerializeField]
    private Transform target;

    // This enemy's pathfinder script
    [SerializeField]
    private Seeker seeker;

    // This enemy's Rigidbody component
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    // Enemy health bar
    private HealthBar healthBar;

    // This enemy's contact collider
    [SerializeField]
    private Collider2D contactColl;

    [Space(10)]
    [Header("ENEMY STATS")]

    // This enemy's attack damage
    [SerializeField]
    private float damage;

    // This enemy's movement speed
    [SerializeField]
    private float speed;

    // This enemy's attack speed
    [SerializeField]
    private float attackSpeed;

    // Boolean to determine whether attack animation is playing
    private bool attackAnim;

    [Space(10)]
    [Header("PATHFINDING")]

    // This enemy's pathfinding waypoint distance
    [SerializeField]
    private float nextWaypointDistance = 3f;

    Pathfinding.Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    void Start() {
        if (healthBar == null) {
            Debug.Log("ContactEnemy healthbar is null! Reassigned.");
            healthBar = this.GetComponentInChildren<HealthBar>();
        }

        if (seeker == null) {
            Debug.Log("ContactEnemy seeker is null! Reassigned.");
            seeker = GetComponent<Seeker>();
        }
        if (rb == null) {
            Debug.Log("ContactEnemy rb is null! Reassigned.");
            rb = GetComponent<Rigidbody2D>();
        }
        //if (target == null) {
            //Debug.Log("ContactEnemy target is null! Reassigned.");
            target = GameObject.FindGameObjectWithTag("Player").transform;
        //}

        attackAnim = false;

        InvokeRepeating(nameof(UpdatePath), 0f, .5f);
    }

    void UpdatePath() {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void FixedUpdate() {

        // Pathfinding
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        } else {
            reachedEndOfPath = false;
        }

        // Movement
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        // Direction facing
        if (rb.velocity.x >= 0.01f) {

            this.transform.localScale = new Vector3(1f, 1f, 1f);
            animator.SetBool("IsMoving", true);

        } else if (rb.velocity.y <= -0.01f) {

            this.transform.localScale = new Vector3(-1f, 1f, 1f);
            animator.SetBool("IsMoving", true);

        } else {
            animator.SetBool("IsMoving", false);
        }

        // Make health bar face the same way regardless of enemy sprite
        if (this.transform.localScale == new Vector3(1f, 1f, 1f)) {

            healthBar.transform.localScale = new Vector3(1f, 1f, 1f);

        } else if (this.transform.localScale == new Vector3(-1f, 1f, 1f)) {

            healthBar.transform.localScale = new Vector3(-1f, 1f, 1f);

        }

        // Resets attack animation if not colliding with anything
        if (contactColl.enabled == true) {
            animator.SetBool("Attack", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        // If collided with the player, start attack sequence
        if (collider.CompareTag("Player")) {

            // If animation has started playing
            if (attackAnim == true) {

                // Disable collider and animation trigger to prevent looping
                contactColl.enabled = false;
                animator.SetBool("Attack", false);

                // Damage entity
                StartCoroutine(AttackEntity(collider));
            }

            // Play attack animation (if not already playing)
            else {
                attackAnim = false;
                contactColl.enabled = false;
                animator.SetBool("Attack", true);
            }

        } 
        // If not collided with the player, reset attack sequence
        else {
            attackAnim = false;
            animator.SetBool("Attack", false);
            contactColl.enabled = true;
        }
    }

    // Ends the attack animation (RUNS AT THE LAST FRAME OF ANIMATION)
    public void CheckTrigger() {
        attackAnim = true;
        animator.SetBool("Attack", false);
        contactColl.enabled = true;
    }

    private IEnumerator AttackEntity(Collider2D target) {
        // Deal damage to enemy                        
        if (target.TryGetComponent<PlayerController>(out var player)) {
            player.TakeDamage(damage);
        } else {
            Debug.LogError("Tried to damage nonexistent entity! Or the entity has no collider.");
        }
        attackAnim = false;

        // Wait for attack cooldown
        yield return new WaitForSeconds(attackSpeed);

        contactColl.enabled = true;
    }

    void OnPathComplete(Pathfinding.Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void RemoveEnemy() {
        Destroy(gameObject);
    }
}
