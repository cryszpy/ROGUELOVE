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
            target = player.transform.position;
            Chase();
        } 
        // If player IS in follow radius, then stand still
        else if (inFollowRadius) {
            seen = true;
            canWander = false;
            force = Vector2.zero;
            target = player.transform.position;
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