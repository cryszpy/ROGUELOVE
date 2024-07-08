using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBreakWall : MonoBehaviour
{

    // Parent class
    [SerializeField]
    protected Enemy parent;
    
    public void OnCollisionEnter2D(Collision2D collider) {
        var grid = parent.map.floorTilemap.layoutGrid;
        Vector3 hitPosition = Vector3.zero;

        foreach (ContactPoint2D contact in collider.contacts) {
            hitPosition.x = contact.point.x - 0.01f * contact.normal.x;
            hitPosition.y = contact.point.y - 0.01f * contact.normal.y;

            var cellPos = grid.WorldToCell(hitPosition);

            //Debug.DrawLine(hitPosition, Vector3.zero, Color.white, 3);
            //Debug.Log(hitPosition);
            //Debug.Log("Cell:" + grid.WorldToCell(hitPosition));

            if (parent.map.wallsTilemap.GetTile(new Vector3Int(cellPos.x, cellPos.y)) == parent.map.tiles.walls) {
                parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y), parent.map.tiles.empty);
                AstarPath.active.UpdateGraphs(parent.map.wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
                Debug.Log("Updated graph bounds because of broken block!");
            } 
            else if (parent.map.wallsTilemap.GetTile(new Vector3Int(cellPos.x, cellPos.y + 1)) == parent.map.tiles.walls) {
                parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y + 1), parent.map.tiles.empty);
                AstarPath.active.UpdateGraphs(parent.map.wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
                Debug.Log("Updated graph bounds because of broken block!");
            } 
            else if (parent.map.wallsTilemap.GetTile(new Vector3Int(cellPos.x + 1, cellPos.y)) == parent.map.tiles.walls) {
                parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x + 1, cellPos.y), parent.map.tiles.empty);
                AstarPath.active.UpdateGraphs(parent.map.wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
                Debug.Log("Updated graph bounds because of broken block!");
            }
            else if (parent.map.wallsTilemap.GetTile(new Vector3Int(cellPos.x, cellPos.y - 1)) == parent.map.tiles.walls) {
                parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y - 1), parent.map.tiles.empty);
                AstarPath.active.UpdateGraphs(parent.map.wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
                Debug.Log("Updated graph bounds because of broken block!");
            } 
            else if (parent.map.wallsTilemap.GetTile(new Vector3Int(cellPos.x - 1, cellPos.y)) == parent.map.tiles.walls) {
                parent.map.wallsTilemap.SetTile(new Vector3Int(cellPos.x - 1, cellPos.y), parent.map.tiles.empty);
                AstarPath.active.UpdateGraphs(parent.map.wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
                Debug.Log("Updated graph bounds because of broken block!");
            } else {
                Debug.LogWarning("Could not find wall for " + parent.gameObject.name + " to break!");
            }
        }
    }
}
