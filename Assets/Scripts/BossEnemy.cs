using UnityEngine;

public class BossEnemy : Enemy
{

    public override void SetEnemyType() {
        enemyType = EnemyType.BOSS;
    }

    public override void Start()
    {
        base.Start();

        // Start this boss's cutscene
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {

        }
    }

    public override void PlayerCheck() {

        // If player is NOT in follow radius then chase
        if (!inFollowRadius) {
            seen = true;
            canWander = false;
            force = Vector2.zero;
            target = player.position;
            Chase();
        } 
        // If player IS in follow radius, then stand still
        else if (inFollowRadius) {
            seen = true;
            canWander = false;
            force = Vector2.zero;
            target = player.position;
        }
    }

    public override void RollAttacks()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING && enemyType != EnemyType.DEAD) {
            
            // If the enemy can fire—
            if (canFire) {

                // If player is close enough—
                if (inFollowRadius) {

                    // Roll for close-quarters combat
                    CloseAttack();

                }
                // Otherwise use distance attack
                else {
                    // Roll for long-distance combat
                    DistanceAttack();
                }

            }
        }
    }

    public virtual void CloseAttack()
    {
        float roll = UnityEngine.Random.value;

        bool fired = false;

        // For every possible attack this enemy has—
        foreach (var attack in attacksList) {

            // If the attack is ranged—
            if (attack.attackType == EnemyAttackType.CLOSE) {

                // If the attack's success roll is successful—
                if (roll <= attack.attackChance) {

                    // Use the attack
                    currentAttack = attack;
                    attack.FiringMethod();

                    fired = true;

                    // Don't use any other attack
                    break;
                }
            }
        }

        // If all possible attacks failed the roll, use the first attack in the list.
        if (!fired) {
            DistanceAttack();
        }
    }

    public override void DistanceAttack()
    {
        float roll = UnityEngine.Random.value;

        bool fired = false;

        // For every possible attack this enemy has—
        foreach (var attack in attacksList) {

            // If the attack is ranged—
            if (attack.attackType == EnemyAttackType.DISTANCE) {

                // If the attack's success roll is successful—
                if (roll <= attack.attackChance) {

                    // Use the attack
                    currentAttack = attack;
                    attack.FiringMethod();

                    fired = true;

                    // Don't use any other attack
                    break;
                }
            }
        }

        // If all possible attacks failed the roll, use the first attack in the list.
        if (!fired) {
            currentAttack = attacksList[0];
            attacksList[0].FiringMethod();
        }
    }

    public override void DirectionFacing()
    {

        if (target.x - this.transform.position.x >= 0f) {
            spriteRenderer.flipX = false;
        } else if (target.x - this.transform.position.x < 0f) {
            spriteRenderer.flipX = true;
        }
    }

    // Called at the START of this enemy's death animation
    public override void EnemyDeath() {

        // Sets enemy type to DEAD
        enemyType = EnemyType.DEAD;

        GameStateManager.EOnEnemyDeath?.Invoke();

        // Sets force to 0 so that the enemy doesn't just fly off
        force = 0 * Time.fixedDeltaTime * direction;

        // Spawns EXP
        SpawnExp();
        SpawnDrops();
    }

    // Called at the END of this enemy's death animation
    public void PostBossDeath() {

        // Increments dead enemy counter
        WalkerGenerator.AddDeadEnemy();
        Debug.Log(WalkerGenerator.GetDeadEnemies() + "/" + WalkerGenerator.EnemyTotal);
    }
}