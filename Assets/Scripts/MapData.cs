using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MapData
{

    // Array of all tile types (FLOOR, EMPTY, DECOR)
    public int[] tileTypes;

    // Array of all obstacle tile types (OBSTACLES)
    public int[] oTileTypes;

    public int levelNum;

    public int stageNum;

    public MapData (WalkerGenerator map) {

        levelNum = GameStateManager.GetLevel();
        stageNum = GameStateManager.GetStage();

        tileTypes = new int[map.gridHandler.GetLength(0) * map.gridHandler.GetLength(1)];
        oTileTypes = new int[map.gridHandler.GetLength(0) * map.gridHandler.GetLength(1)];

        int listNum = 0;
        
        // For each tile position (x, y) within the bounds of the tilemap (e.g. 35x35)
        for (int x = 0; x < map.gridHandler.GetLength(0); x++) {

            for (int y = 0; y < map.gridHandler.GetLength(1); y++) {

                //TileBase type = map.floorTilemap.GetTile(new Vector3Int(x, y, 0));
                //TileBase oType = map.oTilemap.GetTile(new Vector3Int(x, y, 0));
                int type = (int)map.gridHandler[x, y];
                TileBase oType = map.oTilemap.GetTile(new Vector3Int(x, y, 0));

                //if (type != null) {
                if (type == (int)TileType.FLOOR) {
                    tileTypes[listNum] = 1;
                } 
                /*else if (type == map.tiles.decor) {
                    tileTypes[listNum] = 2;
                }*/ 
                else if (type == (int)TileType.WALLS) {
                    tileTypes[listNum] = 2;
                }
                //}

                if (oType != null) {
                    if (oType == map.tiles.obstacles) {
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
