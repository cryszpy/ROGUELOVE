using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;
using NUnit.Framework.Constraints;
using System;
using Unity.VisualScripting;

public class RangedEnemy : Enemy
{
    private EnemyType type = EnemyType.RANGED;

    public override void PlayerCheck() {
        if (canWander && !waiting) {
            //Debug.Log("STARTED WANDERING");
            target = player.position;
            canWander = false;
            Wander();
        }
    }

    public override void DirectionFacing()
    {

        if (target.x - this.transform.position.x >= 0f) {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        } else if (target.x - this.transform.position.x < 0f) {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        base.DirectionFacing();
    }
}