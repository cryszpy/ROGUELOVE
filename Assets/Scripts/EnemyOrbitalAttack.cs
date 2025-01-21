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

            Tuple<int, int> tile = null;

            // Always make sure first shot of attack aims at player
            if (i == 0) {
                tile = parent.map.GetNearestGroundTile(uncheckedTiles, parent.player.transform.position);
            } else {
                tile = parent.map.GetGroundTileWithRadius(uncheckedTiles);
            }

            // Spawns attack
            if (tile != null && weapon.ammo.TryGetComponent<EnemyOrbitalBullet>(out var script)) {
                GameObject attack = script.Create(weapon.ammo, new(tile.Item1 * parent.map.mapGrid.cellSize.x, tile.Item2 * parent.map.mapGrid.cellSize.y), parent) as GameObject;
            } else {
                Debug.LogError("Chosen tile is null and/or could not get EnemyOrbitalAttack script from parent ammo!");
                break;
            }

            // Waits for specified amount of time between bullets in burst
            yield return new WaitForSeconds(timeBetweenBulletBurst);
        }

        // If the enemy doesn't have 0 ranged cooldown, then use min/max values to randomize the next cooldown
        /* if (enemy.rangedAttackCooldownMin != 0 && enemy.rangedAttackCooldownMax != 0) {
            enemy.attackCooldown = UnityEngine.Random.Range(enemy.rangedAttackCooldownMin, enemy.rangedAttackCooldownMax);
        } */

        // Reset attacks
        parent.currentAttack = null;
        parent.attacking = false;
        isChargedShot = false;
    }
}
