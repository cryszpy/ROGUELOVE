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

    public override void SetEnemyType() {
        enemyType = EnemyType.RANGED;
    }

    public override void PlayerCheck() {

        if (!timerSet) {
            // Sets the amount of time spent moving
            moveTime = UnityEngine.Random.Range(3, 5);

            // Sets a cooldown before wandering again
            waitTime = UnityEngine.Random.Range(5, 7);
            
            timerSet = true;
        }

        // If enemy is not currently taking knockback
        if (!kbEd) {
            
            // If player is not in follow radius and enemy has seen, move towards player
            if (inFollowRadius != true && seen) {
                target = player.position;

                Chase();
            } 
            // If player is in follow radius and enemy has seen but something is blocking, move towards player
            else if (inFollowRadius == true && seen && !hitPlayer) {
                target = player.position;

                Chase();
            }
            // If player has not been spotted, wander around
            else if (!seen && canWander) {

                // Gets target tile
                Vector3 randTile = GetWanderTile();
                
                
                // If target hasn't been set
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