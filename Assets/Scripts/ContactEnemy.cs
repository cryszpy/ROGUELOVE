using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;
using NUnit.Framework.Constraints;
using System;

public class ContactEnemy : Enemy
{
    //protected Vector3 tile;

    public override void SetEnemyType() {
        enemyType = EnemyType.CONTACT;
    }

    public override void BeginAttack()
    {
        animator.SetBool("Attack", false);
    }

}