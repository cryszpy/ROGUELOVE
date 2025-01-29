using UnityEngine;

public class CATMachineEnemy : ContactEnemy
{

    public Collider2D breakWallColl;

    public override void PlayerCheck() {

        if (!timerSet) {
            // Sets the amount of time spent moving
            moveTime = UnityEngine.Random.Range(3, 5);

            // Sets a cooldown before wandering again
            waitTime = UnityEngine.Random.Range(5, 7);
            
            timerSet = true;
        }

        // If player is in follow radius then chase
        if (inFollowRadius == true && !seen) {
            seen = true;
        } 
        else if (seen) {
            canWander = false;
            force = Vector2.zero;
            target = player.transform.position;
            Chase();
        }
        // If player is not in follow radius, and wander cooldown is reset, then wander
        else if (inFollowRadius == false && canWander && !seen) {

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

    public override void RollAttacks()
    {
        if (inContactColl) {
            base.RollAttacks();
        }
    }

    public virtual void EndBreakWall() {
        animator.SetBool("BreakWall", false);
    }

    // Sprite direction facing
    public override void DirectionFacing() {

        if (!kbEd) {

            if (rb.linearVelocity.x >= 0.001f) {

                transform.localScale = new(1, 1, 1);
                animator.SetBool("IsMoving", true);

            } else if (rb.linearVelocity.x <= -0.001f) {

                transform.localScale = new(-1, 1, 1);
                animator.SetBool("IsMoving", true);

            } else if (rb.linearVelocity.y <= -0.001 || rb.linearVelocity.y >= 0.001) {
                animator.SetBool("IsMoving", true);
            } else {
                animator.SetBool("IsMoving", false);
            }
        }
    }
}