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

        if (inFollowRadius == true && !waiting && canWander) {
            seen = true;
            canWander = false;
            target = player.position;
            Wander();
        } else if (inFollowRadius == false && canWander && !waiting && seen) {
            target = player.position;
            //Debug.Log("STARTED WANDERING");
            //canWander = false;
            Chase();
        }
    }

    // Wander logic
    public override IEnumerator Roam() {

        // Picks a random direction
        direc = UnityEngine.Random.Range(0, 8);

        // Sets the amount of time spent moving
        moveTime = UnityEngine.Random.Range(2, 5);
        //Debug.Log(moveTime);

        // Sets a cooldown before wandering again
        waitTime = UnityEngine.Random.Range(3, 6);
        //Debug.Log(waitTime);

        switch (direc) {
            case 0:
                force = wanderSpeed * Time.deltaTime * Vector2.up;
                yield return null;
                break;
            case 1:
                force = wanderSpeed * Time.deltaTime * Vector2.down;
                yield return null;
                break;
            case 2:
                force = wanderSpeed * Time.deltaTime * Vector2.right;
                yield return null;
                break;
            case 3:
                force = wanderSpeed * Time.deltaTime * Vector2.left;
                yield return null;
                break;
            case 4:
                force = wanderSpeed * Time.deltaTime * Vector2.zero;
                yield return null;
                break;
            case 5:
                force = wanderSpeed * Time.deltaTime * Vector2.up;
                force += wanderSpeed * Time.deltaTime * Vector2.right;
                yield return null;
                break;
            case 6:
                force = wanderSpeed * Time.deltaTime * Vector2.up;
                force += wanderSpeed * Time.deltaTime * Vector2.left;
                yield return null;
                break;
            case 7:
                force = wanderSpeed * Time.deltaTime * Vector2.down;
                force += wanderSpeed * Time.deltaTime * Vector2.right;
                yield return null;
                break;
            case 8:
                force = wanderSpeed * Time.deltaTime * Vector2.down;
                force += wanderSpeed * Time.deltaTime * Vector2.left;
                yield return null;
                break;
            default:
                direc = UnityEngine.Random.Range(0, 4);
                yield return null;
                break;
        }
        //Debug.Log("Set Direction");
        while (canWander == false && !waiting) {
            //Debug.Log("IN THE LOOP");

            // Moves in the set direction for wandering
            rb.AddForce(force);
            yield return null;
        }
        yield return null;
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