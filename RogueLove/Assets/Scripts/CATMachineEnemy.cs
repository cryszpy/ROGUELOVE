using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;
using NUnit.Framework.Constraints;
using System;

public class CATMachineEnemy : ContactEnemy
{

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
            target = player.position;
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


}