using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrbitalAttack : EnemyRangedAttack
{
    // Starts the attack animation and bullet firing
    public override void StartAttackAnim() {

        if (parent.enemyType != EnemyType.DEAD && GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            parent.canFire = false;

            parent.animator.SetBool("OrbitalAttack", true);
        }
    }

    public override IEnumerator Attack() {
        parent.attacking = true;

        // Creates a local copy of the list containing all ground tiles
        List<Tuple<int, int>> uncheckedTiles = new(parent.map.groundTiles);

        // For the specified number of bullets in this burst attack—
        for (int i = 0; i < numberOfBurstShots; i++) {

            // Play firing sound
            if (!string.IsNullOrWhiteSpace(weapon.fireSound)) {
                FireSound();
            }

            Tuple<int, int> tile;

            // Always make sure first shot of attack aims at player
            if (i == 0) {
                tile = parent.map.GetNearestGroundTile(uncheckedTiles, parent.player.transform.position);
            } else {
                tile = parent.map.GetGroundTileWithRadius(uncheckedTiles);
            }

            // Empty GameObject for chosen bullet
            GameObject chosenBullet = null;

            // If weapon has multiple possible ammo bullets—
            if (weapon.ammoList.Count > 1) {

                // Picks a random projectile to spawn
                float rand = UnityEngine.Random.value;

                // Loops through all possible ammo to compare spawn thresholds
                foreach (var ammoStruct in weapon.ammoList) {

                    // If found chosen bullet, set it as the bullet to spawn and exit loop
                    if (rand <= ammoStruct.spawnChanceCutoff) {
                        chosenBullet = ammoStruct.ammo;
                        break;
                    }
                }

                if (chosenBullet == null) {
                    Debug.LogError("Could not find suitable chosen bullet for this weapon!");
                }
            } 
            // If weapon only has one type of ammo—
            else if (weapon.ammoList.Count == 1) {

                chosenBullet = weapon.ammoList[0].ammo;
            }

            // Spawns attack
            if (tile != null && chosenBullet.TryGetComponent<EnemyOrbitalBullet>(out var script)) {
                GameObject attack = script.Create(chosenBullet, new(tile.Item1 * parent.map.mapGrid.cellSize.x, tile.Item2 * parent.map.mapGrid.cellSize.y), parent) as GameObject;
            } else {
                Debug.LogError("Chosen tile is null and/or could not get EnemyOrbitalAttack script from parent ammo!");
                break;
            }

            // Waits for specified amount of time between bullets in burst
            yield return new WaitForSeconds(timeBetweenBulletBurst);
        }

        // Reset attacks
        parent.currentAttack = null;
        parent.attacking = false;
        isChargedShot = false;
    }
}
