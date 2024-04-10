using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;
using NUnit.Framework.Constraints;

public class ContactEnemy : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    // This enemy's Animator component
    public Animator animator;

    // This enemy's target
    [SerializeField]
    public Transform target;

    // This enemy's pathfinder script
    [SerializeField]
    private Seeker seeker;

    // This enemy's followRange collider
    [SerializeField]
    private Collider2D followCollider;

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
    public float damage;

    // This enemy's movement speed
    [SerializeField]
    private float speed;

    // This enemy's attack speed
    public float attackSpeed;

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

    public bool inFollowRadius;

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

        if (inFollowRadius == true) {
            rb.AddForce(force);
        } 
        
        

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

    // Ends the attack animation (RUNS AT THE LAST FRAME OF ANIMATION)
    public void CheckTrigger() {
        //attackAnim = false;
        animator.SetBool("Attack", false);
        //contactColl.enabled = true;
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
