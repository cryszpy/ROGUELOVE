using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    [SerializeField] protected bool inRadius;

    [SerializeField] protected int areaIndex;

    private void OnTriggerEnter2D(Collider2D collider) {

        // If player is within radius, set true
        if (collider.CompareTag("Player")) {
            inRadius = true;
        } 
    }

    private void OnTriggerExit2D(Collider2D collider) {

        // If player leaves radius, set false
        if (collider.CompareTag("Player")) {
            inRadius = false;
        } 
    }

    public virtual void Update() {
        if (inRadius && Input.GetKeyDown(KeyCode.E)) {

            Debug.Log("Entered Door: " + areaIndex);
        }
    }
}
