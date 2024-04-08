using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MapData
{
    // Array of all tile X values
    //public int[] xTileLoc;

    // Array of all tile Y values
    //public int[] yTileLoc;

    // Array of all tile types (FLOOR, EMPTY, DECOR)
    public int[] tileTypes;

    // Array of all obstacle tile types (OBSTACLES)
    public int[] oTileTypes;

    public MapData (WalkerGenerator map) {

        tileTypes = new int[map.gridHandler.GetLength(0) * map.gridHandler.GetLength(1)];
        oTileTypes = new int[map.gridHandler.GetLength(0) * map.gridHandler.GetLength(1)];
        Debug.Log("tileTypes length: " + tileTypes.Length);

        /*
        xTileLoc = new int[map.gridHandler.GetLength(0) * map.gridHandler.GetLength(1)];
        Debug.Log("xTileLoc length: " + xTileLoc.Length);
        yTileLoc = new int[map.gridHandler.GetLength(1) * map.gridHandler.GetLength(0)];
        Debug.Log("yTileLoc length: " + yTileLoc.Length);
        */

        //float[] duh = new float[14] {1, 2, 9, 6, 7, 5, 4, 7, 8, 34, 56, 22, 24, 23};

        int listNum = 0;
        
        // For each tile position (x, y) within the bounds of the tilemap (e.g. 35x35)
        for (int x = 0; x < map.gridHandler.GetLength(0); x++) {

            for (int y = 0; y < map.gridHandler.GetLength(1); y++) {

                //Debug.Log(x);
                //xTileLoc[listNum] = x;
                //yTileLoc[listNum] = y;

                TileBase type = map.tilemap.GetTile(new Vector3Int(x, y, 0));
                TileBase oType = map.oTilemap.GetTile(new Vector3Int(x, y, 0));
                //Sprite sprite = map.tilemap.GetSprite(new Vector3Int(x, y, 0));

                //Debug.Log(type);

                if (type != null) {
                    if (type == map.floor) {
                        tileTypes[listNum] = 1;
                    } else if (type == map.decor) {
                        tileTypes[listNum] = 2;
                    } else if (type == map.empty) {
                        tileTypes[listNum] = 3;
                    }
                }

                if (oType != null) {
                    if (oType == map.obstacles) {
                        oTileTypes[listNum] = 1;
                    }
                }
                listNum++;
            }

        }
        //string delimiter = ", ";

        /*
        string xx = xTileLoc.Select(i => i.ToString()).Aggregate((i, j) => i + delimiter + j);
        Debug.Log(xx);

        string yy = yTileLoc.Select(i => i.ToString()).Aggregate((i, j) => i + delimiter + j);
        Debug.Log(yy);
        

        string tiles = tileTypes.Select(i => i.ToString()).Aggregate((i, j) => i + delimiter + j);
        Debug.Log(tiles);
        */

        //string test = duh.Select(i => i.ToString()).Aggregate((i, j) => i + delimiter + j);
        //Debug.Log(test);
    }
}
