using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;

public class ContactEnemy : MonoBehaviour
{

    // Animator component
    public Animator animator;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float speed = 200f;

    [SerializeField]
    private float nextWaypointDistance = 3f;

    Pathfinding.Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    [SerializeField]
    private Seeker seeker;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    // Enemy health bar
    private HealthBar healthBar;

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
