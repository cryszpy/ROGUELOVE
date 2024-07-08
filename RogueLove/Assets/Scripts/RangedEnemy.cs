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

            // If enemy has direct line of sight with player
            if (hitPlayer && inFollowRadius) {

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
            if (inFollowRadius != true && hitPlayer) {
                target = player.position;

                Chase();
            } 
            // If enemy has direct line of sight with player (and follow radius does not matter)
            else if (hitPlayer && canWander) {
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
            else if (!hitPlayer && canWander) {

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
            if (rb.velocity.x >= 0.001f) {

                this.transform.localScale = new Vector3(1f, 1f, 1f);
                animator.SetBool("IsMoving", true);

            } 
            else if (rb.velocity.x <= -0.001f) {

                this.transform.localScale = new Vector3(-1f, 1f, 1f);
                animator.SetBool("IsMoving", true);

            } 
            else if (rb.velocity.y <= -0.001 || rb.velocity.y >= 0.001) {
                animator.SetBool("IsMoving", true);
            } 
            // Else if player is in direct line of sight, face player
            else if (hitPlayer) {
                if (player.transform.position.x > this.transform.position.x) {
                    this.transform.localScale = new Vector3(1f, 1f, 1f);
                } 
                else if (player.transform.position.x < this.transform.position.x) {
                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
            }
            else {
                animator.SetBool("IsMoving", false);
            }         

            // Make health bar face the same way regardless of enemy sprite
            if (this.transform.localScale == new Vector3(1f, 1f, 1f)) {

                healthBar.transform.localScale = new Vector3(1f, 1f, 1f);

            } else if (this.transform.localScale == new Vector3(-1f, 1f, 1f)) {

                healthBar.transform.localScale = new Vector3(-1f, 1f, 1f);

            }
        }




        
        // If player is NOT in direct line of sight, face whatever target block the enemy is wandering to
        /* else {

            if (target.x - this.transform.position.x >= 0f) {
                this.transform.localScale = new Vector3(1f, 1f, 1f);
            } 
            else if (target.x - this.transform.position.x < 0f) {
                this.transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            base.DirectionFacing();
        } */
    }
}