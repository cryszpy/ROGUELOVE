using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceCollectionRadius : EnemyFollowRadius
{
    public override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) {
            parent.RemoveEnemy();
        }
    }
}
