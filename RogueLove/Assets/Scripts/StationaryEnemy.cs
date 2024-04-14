using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;
using NUnit.Framework.Constraints;
using System;

public class StationaryEnemy : Enemy
{

    public override void SetEnemyType()
    {
        enemyType = EnemyType.STATIONARY;
    }


}