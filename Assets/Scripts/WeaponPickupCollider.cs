using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponPickupCollider : MonoBehaviour
{
    
    [SerializeField] private Rigidbody2D rb;

    private bool colliding;

    private Vector3 direction = new(0, 0);

    private List<GameObject> collidingObjects = new();

    private void Update() {

        if (colliding) {
            if (direction == Vector3.zero) {

                Vector3 newDirection = Vector2.zero;

                int rand = Random.Range(0, 2);

                switch (rand) {
                    case 0:
                        newDirection = new(-1, 0);
                        break;
                    case 1:
                        newDirection = new(1, 0);
                        break;
                }
                
                rb.AddForce(newDirection.normalized);
            } else {
                rb.AddForce(-direction.normalized);
            }
        } else {
            direction = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll) {

        if (coll.gameObject.layer == LayerMask.NameToLayer("Pickup")) {
            colliding = true;
            
            if (!collidingObjects.Contains(coll.gameObject)) {
                collidingObjects.Add(coll.gameObject);
                direction += coll.gameObject.transform.position - rb.gameObject.transform.position;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D coll) {

        if (coll.gameObject.layer == LayerMask.NameToLayer("Pickup")) {
            colliding = true;

            if (!collidingObjects.Contains(coll.gameObject)) {
                collidingObjects.Add(coll.gameObject);
                direction += coll.gameObject.transform.position - rb.gameObject.transform.position;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D coll) {

        if (coll.gameObject.layer == LayerMask.NameToLayer("Pickup")) {
            colliding = false;

            if (collidingObjects.Contains(coll.gameObject)) {
                collidingObjects.Remove(coll.gameObject);
                direction -= coll.gameObject.transform.position - rb.gameObject.transform.position;
            }
        }
    }
}
