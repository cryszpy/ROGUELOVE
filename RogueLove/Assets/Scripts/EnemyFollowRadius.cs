using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyFollowRadius : MonoBehaviour
{
    [SerializeField]
    private ContactEnemy parent;

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.CompareTag("Player")) {
            parent.inFollowRadius = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if(collider.CompareTag("Player")) {
            parent.inFollowRadius = false;
        }
    }
}
