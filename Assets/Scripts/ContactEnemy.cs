using UnityEngine;

public class ContactEnemy : Enemy
{
    //protected Vector3 tile;

    public override void SetEnemyType() {
        enemyType = EnemyType.CONTACT;
    }

    public override void RollAttacks()
    {
        if (inContactColl) {
            base.RollAttacks();
        }
    }
}