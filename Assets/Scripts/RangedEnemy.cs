using UnityEngine;

public class RangedEnemy : Enemy
{

    public override void SetEnemyType() {
        enemyType = EnemyType.RANGED;
    }

    public override void PlayerCheck() {

        if (!timerSet) {

            // If enemy has direct line of sight with player
            if (seesPlayer && inFollowRadius) {

                // Sets the amount of time spent moving
                moveTime = UnityEngine.Random.Range(0.5f, 1f);

                // Sets a cooldown before wandering again
                waitTime = UnityEngine.Random.Range(1, 2);
                
                timerSet = true;
            } else {
                // Sets the amount of time spent moving
                moveTime = UnityEngine.Random.Range(3, 5);

                // Sets a cooldown before wandering again
                waitTime = UnityEngine.Random.Range(5, 7);
                
                timerSet = true;
            }
        }

        // If enemy is not currently taking knockback
        if (!kbEd) {
            
            // If player is not in follow radius and enemy has direct line of sight, move towards player
            if (inFollowRadius != true && seesPlayer) {
                target = player.transform.position;

                Chase();
            } 
            // If enemy has direct line of sight with player (and follow radius does not matter)
            else if (seesPlayer && canWander) {
                // Gets target tile
                Vector3 randTile = GetCombatWanderTile();

                // If tile hasn't been checked for validity
                if (!tileGot) {
                    tileGot = true;

                    // Set target to tile
                    target = randTile;
                }

                // Wander to tile
                Wander();
            }
            // If enemy does NOT have direct line of sight, wander around
            else if (!seesPlayer && canWander) {

                // Gets target tile
                Vector3 randTile = GetWanderTile();

                // If tile hasn't been checked for validity
                if (!tileGot) {
                    tileGot = true;

                    // Set target to tile
                    target = randTile;
                }

                // Wander to tile
                Wander();
            }
        }
    }

    public override Vector3 GetWanderTile() {
        
        // Picks a random tile within radius
        float tileX = UnityEngine.Random.Range(this.transform.position.x - followCollider.radius, 
            this.transform.position.y + followCollider.radius);

        float tileY = UnityEngine.Random.Range(this.transform.position.y - followCollider.radius, 
            this.transform.position.y + followCollider.radius);

        Vector3 tile = new Vector3(tileX, tileY);

        if (map.CheckGroundTile(tile)) {
            return tile;
        } else {
            return transform.position;
        }
    }

    public Vector3 GetCombatWanderTile() {
        
        // Picks a random tile within radius
        float tileX = UnityEngine.Random.Range(this.transform.position.x - (followCollider.radius / 2), 
            this.transform.position.y + (followCollider.radius / 2));

        float tileY = UnityEngine.Random.Range(this.transform.position.y - (followCollider.radius / 2), 
            this.transform.position.y + (followCollider.radius / 2));

        Vector3 tile = new Vector3(tileX, tileY);

        if (map.CheckGroundTile(tile)) {
            return tile;
        } else {
            return transform.position;
        }
    }

    public override void DirectionFacing()
    {

        if (!kbEd) {

            // If moving in a direction, face that direction
            if (rb.linearVelocity.x >= 0.001f) {

                spriteRenderer.flipX = false;
                animator.SetBool("IsMoving", true);

            } 
            else if (rb.linearVelocity.x <= -0.001f) {

                spriteRenderer.flipX = true;
                animator.SetBool("IsMoving", true);

            } 
            else if (rb.linearVelocity.y <= -0.001 || rb.linearVelocity.y >= 0.001) {
                animator.SetBool("IsMoving", true);
            } 
            // Else if player is in direct line of sight, face player
            else if (seesPlayer) {
                if (player.transform.position.x > this.transform.position.x) {
                    spriteRenderer.flipX = false;
                } 
                else if (player.transform.position.x < this.transform.position.x) {
                    spriteRenderer.flipX = true;
                }
            }
            else {
                animator.SetBool("IsMoving", false);
            }         
        }
    }
}