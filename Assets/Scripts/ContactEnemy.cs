using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;

public class ContactEnemy : MonoBehaviour
{
    public Animator animator;

    [SerializeField]
    private Transform target;

    public float speed = 200f;

    public float nextWaypointDistance = 3f;

    Pathfinding.Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;

    Rigidbody2D rb;

    public float Health {
        set {
            health = value;
            if(health <= 0) {
                Death();
            }
        }

        get {
            return health;
        }
    }

    public float health = 6;

    void Start() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath() {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void FixedUpdate() {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        } else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        if (rb.velocity.x >= 0.01f) {

            this.transform.localScale = new Vector3(1f, 1f, 1f);
            animator.SetBool("IsMoving", true);

        } else if (rb.velocity.y <= -0.01f) {

            this.transform.localScale = new Vector3(-1f, 1f, 1f);
            animator.SetBool("IsMoving", true);

        } else {
            animator.SetBool("IsMoving", false);
        }
    }

    void OnPathComplete(Pathfinding.Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void HurtCheck() {
        animator.SetBool("Hurt", false);
    }

    public void Death() {
        animator.SetTrigger("Death");
    }

    public void RemoveEnemy() {
        Destroy(gameObject);
    }
}
