using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBreakWall : MonoBehaviour
{

    // Parent class
    [SerializeField]
    protected Enemy parent;

    private bool canCheck = true;

    [SerializeField]
    private float checkTime;

    private float checkTimer = 0;

    void FixedUpdate() {

        if (!canCheck) {
            checkTimer += Time.fixedDeltaTime;
            //Debug.Log("wanderTimer: " + wanderTimer);
            if(checkTimer > checkTime) {
                //Debug.Log("Done With WanderTimer");
                canCheck = true;
                checkTimer = 0;
            }
        }
    }
    
    /* public void OnCollisionEnter2D(Collision2D collider) {
        CheckCollisions(collider);
    } */

    public void OnCollisionStay2D(Collision2D collider) {
        if (canCheck && GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            canCheck = false;
            parent.animator.SetBool("Attack", true);
            CheckCollisions(collider);
        }
    }

    public void CheckCollisions(Collision2D collider) {
        var grid = parent.map.floorTilemap.layoutGrid;
        Vector3 hitPosition = Vector3.zero;

        foreach (ContactPoint2D contact in collider.contacts) {
            hitPosition.x = contact.point.x - 0.01f * contact.normal.x;
            hitPosition.y = contact.point.y - 0.01f * contact.normal.y;

            var cellPos = grid.WorldToCell(hitPosition);
            var cellVec = new Vector3(cellPos.x * parent.map.mapGrid.cellSize.x, cellPos.y * parent.map.mapGrid.cellSize.y);

            //Debug.DrawLine(cellVec, parent.player.transform.position, Color.white, 3);
            //Debug.Log(cellPos);
            //Debug.Log("Cell:" + grid.WorldToCell(hitPosition));

            Vector3Int vec = new(cellPos.x, cellPos.y);

            if (cellPos.x + 1 <= parent.map.mapWidth || cellPos.y + 1 <= parent.map.mapHeight) {
                if (parent.map.gridHandler[cellPos.x, cellPos.y] == TileType.WALLS || parent.map.gridHandler[cellPos.x, cellPos.y] == TileType.OBSTACLES) {
                    parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y), parent.map.tiles.empty);
                } 
                else if (parent.map.gridHandler[cellPos.x, cellPos.y + 1] == TileType.WALLS || parent.map.gridHandler[cellPos.x, cellPos.y + 1] == TileType.OBSTACLES) {
                    parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y + 1), parent.map.tiles.empty);
                } /*
                else if (parent.map.gridHandler[cellPos.x + 1, cellPos.y] == TileType.WALLS || parent.map.gridHandler[cellPos.x + 1, cellPos.y] == TileType.OBSTACLES) {
                    parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x + 1, cellPos.y), parent.map.tiles.empty);
                }
                else if (parent.map.gridHandler[cellPos.x, cellPos.y - 1] == TileType.WALLS || parent.map.gridHandler[cellPos.x, cellPos.y - 1] == TileType.OBSTACLES) {
                    parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y - 1), parent.map.tiles.empty);
                } 
                else if (parent.map.gridHandler[cellPos.x - 1, cellPos.y] == TileType.WALLS || parent.map.gridHandler[cellPos.x - 1, cellPos.y] == TileType.OBSTACLES) {
                    parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x - 1, cellPos.y), parent.map.tiles.empty);
                }  */
                else {
                    Debug.LogWarning("Could not find wall for " + parent.gameObject.name + " to break!");
                }
            } else {
                Debug.LogWarning("Tried to break wall out of map bounds!");
            }
        }
        AstarPath.active.UpdateGraphs(parent.map.wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
        Debug.Log("Updated graph bounds because of broken block!");
    }
}
