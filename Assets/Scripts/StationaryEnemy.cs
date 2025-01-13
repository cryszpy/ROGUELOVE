
public class StationaryEnemy : Enemy
{
    public override void SetEnemyType() {
        enemyType = EnemyType.STATIONARY;
    }

    public override void BeginAttack()
    {
        animator.SetBool("Attack", false);
    }

}