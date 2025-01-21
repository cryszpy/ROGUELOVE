using UnityEngine;

public class StationaryEnemy : Enemy
{
    public override void SetEnemyType() {
        enemyType = EnemyType.STATIONARY;
        seen = true;
        seesPlayer = true;
    }

    public override void RollAttacks()
    {
        if (inContactColl) {
            base.RollAttacks();
        }
    }
}