using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{

    public Grid mapGrid;
    
    // 2D Array
    public TileType[,] gridHandler;

    public List<int> tileListX = new();

    public List<int> tileListY = new();

    // List of all active Walkers
    private List<WalkerObject> walkers;

    public Camera mainCam;

    [Header("TILEMAP OBJECTS")]

    // TILEMAP REFERENCES
    public AreaTiles tiles;
    
    public Tilemap floorTilemap;

    public Tilemap wallsTilemap;

    public Tilemap oTilemap;

    [SerializeField] private ChestList chestList;

    [Space(10)]
    [Header("MAP SETTINGS")]

    [Range(7, 60)]
    // Map maximum width <-->
    public int mapWidth;

    [Range(7, 60)]
    // Map maximum height ^v
    public int mapHeight;

    [SerializeField]
    [Range(1, 10)]
    // Maximum amount of active walkers
    private int maxWalkers;

    [SerializeField]
    // Chance for a single tile to not generate an obstacle
    private int emptyChance;

    // Current tile count
    public int tileCount = default;

    // Current obstacle tile count
    public int oTileCount = default;

    [SerializeField]
    [Range(0.1f, 0.98f)]
    // Compares the amount of floor tiles to the percentage of the total grid covered (in floor tiles)
    // NEVER SET THIS TO 1 OR ELSE UNITY WILL CRASH :skull:
    private float fillPercentage;

    [SerializeField]
    // Generation method, pause time between each successful movement
    private float waitTime;

    // SAVE FILE PATH
    private string pathMap;

    // Boolean to see if save file exists
    private bool loadFromSave;

    [SerializeField] private int lvl;

    [SerializeField] private int stg;

    [SerializeField] private float spawnRadiusX;
    
    [SerializeField] private float spawnRadiusY;

    private bool doorwaySpawned;

    [Space(10)]
    [Header("ENTITIES")]

    // Finds player
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cameraLookAt;
    private PlayerController playerCont;

    // List of all common enemies in this level
    public GameObject[] commonEnemies;

    // List of all rare enemies in this level
    public GameObject[] rareEnemies;

    // List of all stationary enemies in this level
    public GameObject[] stationEnemies;

    // List of all minibosses in this level
    public GameObject[] minibosses;

    // List of all bosses in this level
    public GameObject[] bosses;

    [Space(10)]
    [Header("LEVEL INFO")]

    private static int deadEnemies;
    public static void SetDeadEnemy() {
        deadEnemies++;
    }
    public static int GetDeadEnemies() {
        return deadEnemies;
    }

    private static int enemyTotal;

    public static int EnemyTotal { 
        get => enemyTotal; 

        set {
            enemyTotal = value;
        }
    }

    [SerializeField] private bool bossLevel = false;

    [SerializeField] private bool minibossSpawned = false;

    public static bool doneWithLevel = false;

    // Initializes grid to be generated (size)
    void Awake() {
        doneWithLevel = false;
        enemyTotal = 0;
        deadEnemies = 0;
        GameStateManager.SetLevelClear(false);
        GameStateManager.SetState(GAMESTATE.PLAYING);

        if (GameStateManager.GetLevel() == 1) {
            minibossSpawned = false;
        }

        if (floorTilemap == null) {
            floorTilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        }
        if (wallsTilemap == null) {
            wallsTilemap = GameObject.FindGameObjectWithTag("WallsTilemap").GetComponent<Tilemap>();
        }
        if (oTilemap == null) {
            oTilemap = GameObject.Find("Obstacles").GetComponent<Tilemap>();
        }
        if (tiles == null) {
            tiles = GameObject.FindGameObjectWithTag("AreaTiles").GetComponent<AreaTiles>();
        }
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerCont = player.GetComponent<PlayerController>();
        }
        if (cameraLookAt == null) {
            cameraLookAt = GameObject.FindGameObjectWithTag("CameraLookAt");
        }

        pathMap = Application.persistentDataPath + "/map.chris";

        // Load player info from saved game
        if (File.Exists(pathMap) && GameStateManager.SavePressed()) {
            Debug.Log("ONE");
            Debug.Log("SAVE EXISTS WHAIOGFJAWIOFJIAWFL");
            loadFromSave = true;
        } 
        // Save data exists but player did not click load save --> most likely a NextLevel() call
        else if (File.Exists(pathMap) && GameStateManager.SavePressed() == false) {
            Debug.Log("TWO");
        }
        // Save data does not exist, and player clicked load save somehow
        else if (!File.Exists(pathMap) && GameStateManager.SavePressed() == true) {
            Debug.LogError("Saved map data not found while trying to load save. How did you get here?");
        }
        // Save data does not exist and player did not click load save --> most likely started new game
        else if (!File.Exists(pathMap) && GameStateManager.SavePressed() == false) {
            Debug.Log("STARTED NEW GAME");
            loadFromSave = false;
        }
        else {
            Debug.Log("THREE");
        }

        // If at the main menu and just starting a game, set stage and level to 1
        if (GameStateManager.GetLevel() == 0) {
            GameStateManager.SetLevel(1);
        }
        if (GameStateManager.GetStage() == 0) {
            GameStateManager.SetStage(1);
        }

        lvl = GameStateManager.GetLevel();
        stg = GameStateManager.GetStage();

        // Reset movedDialogue boolean to make sure starting dialogue plays after dialogue is queued
        GameStateManager.dialogueManager.dialogueList.movedDialogue = false;

        Debug.Log("Attempting to initialize grid");
        InitializeGrid();

        if (loadFromSave == true) {
            LoadMap();
        }
    }

    void Update() {
        if (EnemyTotal != 0) {
            if (EnemyTotal == GetDeadEnemies() && GameStateManager.GetLevelClear() == false) {
                GameStateManager.SetLevelClear(true);
                SpawnDoorway();
            }
        }

        if (doneWithLevel && GameStateManager.dialogueManager.dialogueList.priority != null && GameStateManager.dialogueManager.dialogueList.movedDialogue) {

            doneWithLevel = false;

            Debug.Log("Triggered Dialogue");
            
            // Plays the first dialogue in the priority queue (last entry in the list)
            GameStateManager.dialogueManager.StartDialogue(GameStateManager.dialogueManager.dialogueList.priority[^1]);
        }
    }
    void InitializeGrid() {

        // Initializes grid size from variables
        gridHandler = new TileType[mapWidth, mapHeight];
        tileCount = 0;

        // Loops through grid size and sets every tile to EMPTY tile
        for (int x = 0; x < gridHandler.GetLength(0); x++) {
            for (int y = 0; y < gridHandler.GetLength(1); y++) {
                gridHandler[x,y] = TileType.EMPTY;
            }
        }

        // New instance of the WalkerObject list
        walkers = new List<WalkerObject>();

        // Creates reference of the exact centerpiece of the tilemap
        Vector3Int tileCenter = new(0, 0, 0);

        // Creates a new walker, and sets initial values
        WalkerObject currWalker = new WalkerObject(new Vector2(tileCenter.x, tileCenter.y), GetDirection(), 0.5f);

        // Sets current grid location to floor
        gridHandler[tileCenter.x, tileCenter.y] = TileType.FLOOR;
        floorTilemap.SetTile(tileCenter, tiles.floor);

        // Adds current walker to Walker list
        walkers.Add(currWalker);

        // Increases total tile count
        tileCount++;

        if (loadFromSave != true) {
            Debug.Log("CREATED NEW MAP");

            // Handles walker rules
            CreateFloors();
        }
    }

    // Returns a random single vector direction
    Vector2 GetDirection() {
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (choice) {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }
    
    // ^^ same thing but updates direction instead of deletes
    void ChanceToRedirect() {
        for (int i = 0; i < walkers.Count; i++) {
            if (UnityEngine.Random.value < walkers[i].chanceToChange) {
                WalkerObject currWalker = walkers[i];
                currWalker.direction = GetDirection();
                walkers[i] = currWalker;
            }
        }
    }

    // ^^ same thing but creates a new walker (duplicates at position) as long as it is under max walkers
    void ChanceToCreate() {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++) {
            if (UnityEngine.Random.value < walkers[i].chanceToChange && walkers.Count < maxWalkers) {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = walkers[i].position;

                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.5f);
                walkers.Add(newWalker);
            }
        }
    }

    // Update actual position of the walkers, within the bounds of the grid size
    void UpdatePosition() {
        for (int i = 0; i < walkers.Count; i++) {
            WalkerObject foundWalker = walkers[i];
            foundWalker.position += foundWalker.direction;
            foundWalker.position.x = Mathf.Clamp(foundWalker.position.x, 1, gridHandler.GetLength(0) - 2);
            foundWalker.position.y = Mathf.Clamp(foundWalker.position.y, 1, gridHandler.GetLength(1) - 2);
            walkers[i] = foundWalker;
        }
    }
    
    // Loops through walker list, randomly compares a value to the WalkersChance, and if it's > 1 then 
    // deactivates walker if true
    void ChanceToRemove() {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++) {
            if (UnityEngine.Random.value < walkers[i].chanceToChange && walkers.Count > 1) {
                walkers.RemoveAt(i);
                break;
            }
        }
    }

    private void CreateFloors() {
        // Compares tile count as a float to the total size of the grid, and will continue looping as long as it is
        // less than the fillPercentage value set earlier
        while ((float)tileCount / (float)gridHandler.Length < fillPercentage) {
                        
            // Loops through every walker in the list, and creates a reference to its current position to
            // check if it is a FLOOR piece
            foreach (WalkerObject currWalker in walkers) {

                Vector3Int currPos = new Vector3Int((int)currWalker.position.x, (int)currWalker.position.y, 0);
                Vector3Int currPos2 = new Vector3Int((int)currWalker.position.x - 1, (int)currWalker.position.y, 0);
                Vector3Int currPos3 = new Vector3Int((int)currWalker.position.x, (int)currWalker.position.y - 1, 0);
                Vector3Int currPos4 = new Vector3Int((int)currWalker.position.x - 1, (int)currWalker.position.y - 1, 0);

                // If the current position is not a floor tile, then set a new FLOOR tile in that position and increment the total tile count
                if (gridHandler[currPos.x, currPos.y] != TileType.FLOOR) {
                    floorTilemap.SetTile(currPos, tiles.floor);
                    tileListX.Add(currPos.x);
                    tileListY.Add(currPos.y);
                    tileCount++;

                    floorTilemap.SetTile(currPos2, tiles.floor);
                    tileListX.Add(currPos2.x);
                    tileListY.Add(currPos2.y);
                    tileCount++;

                    floorTilemap.SetTile(currPos3, tiles.floor);
                    tileListX.Add(currPos3.x);
                    tileListY.Add(currPos3.y);
                    tileCount++;

                    floorTilemap.SetTile(currPos4, tiles.floor);
                    tileListX.Add(currPos4.x);
                    tileListY.Add(currPos4.y);
                    tileCount++;

                    gridHandler[currPos.x, currPos.y] = TileType.FLOOR;
                    gridHandler[currPos.x - 1, currPos.y] = TileType.FLOOR;
                    gridHandler[currPos.x, currPos.y - 1] = TileType.FLOOR;
                    gridHandler[currPos.x - 1, currPos.y - 1] = TileType.FLOOR;

                }
            }

            // Walker methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();
        }
        
        floorTilemap.SetTile(new Vector3Int(0, 0, 0), tiles.empty);
        tileCount--;
        gridHandler[0, 0] = TileType.EMPTY;

        CreateWalls();
        FillFloors();
        CreateBorders();
        if (!bossLevel) {
            CreateObstacles();
        }
        CreateBreakables();
    }

    private void CreateWalls() {

        // For the length of the grid (x)
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++) {

            // For the height of the grid (y)
            for (int y = 0; y < gridHandler.GetLength(1) - 1; y++) {

                // Checks each x and y value of the grid to see if they are floors
                if (gridHandler[x, y] == TileType.EMPTY) {
                    //bool hasCreatedDecor = false;

                    // DECOR CHECK
                    wallsTilemap.SetTile(new Vector3Int(x, y, 0), tiles.walls);
                    gridHandler[x, y] = TileType.WALLS;
                    //hasCreatedDecor = true;
                    tileCount++;
                }
            }
        }
    }

    private void FillFloors() {

        // For the length of the grid (x)
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++) {

            // For the height of the grid (y)
            for (int y = 0; y < gridHandler.GetLength(1) - 1; y++) {

                // Checks each x and y value of the grid to see if they are floors
                if (gridHandler[x, y] == TileType.WALLS) {

                    floorTilemap.SetTile(new Vector3Int(x, y, 0), tiles.floor);
                    tileListX.Add(x);
                    tileListY.Add(y);
                    tileCount++;
                }
            }
        }
    }

    private void CreateBorders() {

        // For the length of the grid (x)
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++) {

            // Create border along top of map
            if (wallsTilemap.GetTile(new Vector3Int(x, gridHandler.GetLength(1) - 2, 0)) == tiles.walls) {

                oTilemap.SetTile(new Vector3Int(x, gridHandler.GetLength(1) - 1, 0), tiles.grassTop);
                gridHandler[x, gridHandler.GetLength(1) - 1] = TileType.BORDER;
                tileCount++;
                Debug.Log("Created top border tile");
            } else {
                wallsTilemap.SetTile(new Vector3Int(x, gridHandler.GetLength(1) - 1, 0), tiles.borderTop);
                gridHandler[x, gridHandler.GetLength(1) - 1] = TileType.BORDER;
                tileCount++;
                Debug.Log("Created top border tile");
            }

            // Create border along bottom of map
            if (wallsTilemap.GetTile(new Vector3Int(x, 1, 0)) == tiles.walls) {

                wallsTilemap.SetTile(new Vector3Int(x, 0, 0), tiles.grass);
                oTilemap.SetTile(new Vector3Int(x, 0, 0), tiles.grass);

                gridHandler[x, 0] = TileType.BORDER;
                tileCount++;
                Debug.Log("Created bottom border tile");
            } else {
                wallsTilemap.SetTile(new Vector3Int(x, 0, 0), tiles.borderDown);
                gridHandler[x, 0] = TileType.BORDER;
                tileCount++;
                Debug.Log("Created bottom border tile");
            }

        }

        // For the height of the grid (y)
        for (int y = 0; y < gridHandler.GetLength(1) - 1; y++) {

            // Create border along left of map
            if (wallsTilemap.GetTile(new Vector3Int(1, y, 0)) == tiles.walls) {

                wallsTilemap.SetTile(new Vector3Int(0, y, 0), tiles.grass);
                oTilemap.SetTile(new Vector3Int(0, y, 0), tiles.grass);

                gridHandler[0, y] = TileType.BORDER;
                tileCount++;
            } else {
                wallsTilemap.SetTile(new Vector3Int(0, y, 0), tiles.borderLeft);
                gridHandler[0, y] = TileType.BORDER;
                tileCount++;
            }

            // Create border along right of map
            if (wallsTilemap.GetTile(new Vector3Int(gridHandler.GetLength(1) - 2, y, 0)) == tiles.walls) {

                oTilemap.SetTile(new Vector3Int(gridHandler.GetLength(1) - 1, y, 0), tiles.grass);
                gridHandler[gridHandler.GetLength(1) - 1, y] = TileType.BORDER;
                tileCount++;
            } else {
                wallsTilemap.SetTile(new Vector3Int(gridHandler.GetLength(1) - 1, y, 0), tiles.borderRight);
                gridHandler[gridHandler.GetLength(1) - 1, y] = TileType.BORDER;
                tileCount++;

            }
        }

        // Set bottom left corner
        wallsTilemap.SetTile(new Vector3Int(0, 0, 0), tiles.grass);
        //oTilemap.SetTile(new Vector3Int(0, 0, 0), tiles.grass);
        gridHandler[0, 0] = TileType.BORDER;

        // Set bottom right corner
        wallsTilemap.SetTile(new Vector3Int(gridHandler.GetLength(0) - 1, 0, 0), tiles.grass);
        //oTilemap.SetTile(new Vector3Int(gridHandler.GetLength(0) - 1, 0, 0), tiles.grass);
        gridHandler[gridHandler.GetLength(0) - 1, 0] = TileType.BORDER;
    }

    // CREATES OBSTACLES
    private void CreateObstacles() {

        // For the length of the grid (x)
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++) {

            // For the height of the grid (y)
            for (int y = 0; y < gridHandler.GetLength(1) - 1; y++) {

                int rand = UnityEngine.Random.Range(0, emptyChance);

                // Checks each x and y value of the grid to see if they are floors
                if (gridHandler[x, y] == TileType.FLOOR) {

                    // If selected tile is a FLOOR tile, and chances are good, create an obstacle
                    if (gridHandler[x, y] == TileType.FLOOR && rand == 0) {
                        oTilemap.SetTile(new Vector3Int(x, y, 0), tiles.obstacles);
                        gridHandler[x, y] = TileType.OBSTACLES;
                        oTileCount++;
                    }
                }
            }
        }
    }

    private void CreateBreakables() {

        // For all different breakables in the level
        for (int b = 0; b < tiles.breakables.Length; b++) {

            // Generate random amount of each breakable
            int breakRange = UnityEngine.Random.Range(6, 10);

            for (int br = 0; br < breakRange; br++) {

                int rand = GetRandomTile();

                // Check all X tiles
                for (int x = 0; x < tileListX.Count; x++) {

                    // If chosen tile is a floor tile, and doesn't have obstacles, then generate breakable
                    if (gridHandler[tileListX[rand], tileListY[rand]] == TileType.FLOOR 
                        && oTilemap.GetTile(new Vector3Int(tileListX[rand], tileListY[rand], 0)) != tiles.obstacles
                    ) {

                        Instantiate(tiles.breakables[b], new Vector3(tileListX[rand] * mapGrid.cellSize.x + (mapGrid.cellSize.x / 2), tileListY[rand] * mapGrid.cellSize.y + (mapGrid.cellSize.y / 2), 0), Quaternion.identity);
                        break;

                    } else {
                        rand = GetRandomTile();
                    }
                }
            }
        }
        SpawnRandomPlayer();
        SpawnChests();
        StartCoroutine(SpawnStuff());
    }

    private void SpawnChests() {

        // Generate random amount of chests per level
        int chestRange = 1;
        
        // For the amount of chests generated per level
        for (int s = 0; s < chestRange; s++) {
            
            // Generates random number to pick chest spawnpoint
            int rand = GetRandomTile();

            // For as many floor tiles as there are in the tilemap:
            for (int i = 0; i < tileListX.Count; i++) {

                // If suitable floor tiles have been found (Ground tiles and no obstacles on those tiles)
                if (gridHandler[tileListX[rand], tileListY[rand]] == TileType.FLOOR) {

                    if (tileListX[rand] <= player.transform.position.x + spawnRadiusX 
                    && tileListX[rand] >= player.transform.position.x - spawnRadiusX) {
                        rand = GetRandomTile();
                    } else if (tileListY[rand] <= player.transform.position.y + spawnRadiusY 
                    && tileListY[rand] >= player.transform.position.y - spawnRadiusY) {
                        rand = GetRandomTile();
                    } else {

                        Instantiate(chestList.weaponChest, new Vector2(tileListX[rand] * mapGrid.cellSize.x, tileListY[rand] * mapGrid.cellSize.y), Quaternion.identity);
                        break;
                    }

                } else {
                    
                    // Generates random number to pick chest spawnpoint
                    rand = GetRandomTile();
                }
            }
        }
    }

    public IEnumerator SpawnStuff() {
        yield return new WaitForSeconds(0.01f);

        PathScan();
        SpawnRandomEnemies();
        SaveMap();
        TransitionManager.EndLeaf(true);
        playerCont.savePressed = true;
    }

    // SPAWN PLAYER
    public void SpawnRandomPlayer() {

        // Generates random number to pick Player spawnpoint
        int randP = GetRandomTile();

        // For as many floor tiles as there are in the tilemap:
        for (int i = 0; i < tileListX.Count; i++) {

            // If suitable floor tiles have been found (Ground tiles and no obstacles on those tiles)
            if (gridHandler[tileListX[i], tileListY[randP]] == TileType.FLOOR 
                && oTilemap.GetTile(new Vector3Int(tileListX[i], tileListY[randP])) != tiles.obstacles) {

                // Spawns Player
                //player.SetActive(true);
                player.transform.position = new Vector2((tileListX[i] * mapGrid.cellSize.x) + 0.08f, (tileListY[randP] * mapGrid.cellSize.y) + 0.02f);
                break;

            } else {
                // Generates random number to pick Player spawnpoint
                randP = UnityEngine.Random.Range(0, tileListX.Count);
            }
        }
    }

    // Spawns doorway to next level after level cleared
    private void SpawnDoorway() {
        doorwaySpawned = false;

        int rand = GetRandomTile();

        // For all tiles in the map
        for (int x = 0; x < tileListX.Count; x++) {

            // If chosen tile is a floor tile, and doesn't have obstacles, then generate breakable
            if (gridHandler[tileListX[rand], tileListY[rand]] == TileType.FLOOR) {

                Vector3 genPos = floorTilemap.CellToWorld(new Vector3Int(tileListX[rand], tileListY[rand]));

                // Makes sure the chosen tile is not near the player (avoids accidentally skipping animation)
                if (genPos.x <= player.transform.position.x + spawnRadiusX 
                    && genPos.x >= player.transform.position.x - spawnRadiusX
                    && genPos.y <= player.transform.position.y + spawnRadiusY 
                    && genPos.y >= player.transform.position.y - spawnRadiusY) {
                        rand = GetRandomTile();
                    } else {

                        // Spawn doorway
                        if (tiles.doorwayObject.TryGetComponent<Doorway>(out var door)) {
                            door.Create(tiles.doorwayObject, new Vector2((tileListX[rand] * mapGrid.cellSize.x) + (mapGrid.cellSize.x / 2), (tileListY[rand] * mapGrid.cellSize.y) + (mapGrid.cellSize.y / 2)), Quaternion.identity, cameraLookAt);
                            doorwaySpawned = true;
                            break;
                        } else {
                            Debug.LogError("Could not find Doorway component of door while spawning!");
                            break;
                        }
                    }
            } 
            else {
                rand = GetRandomTile();
            }
        }
        if (!doorwaySpawned) {
            Debug.LogWarning("Could not find suitable tile to spawn doorway.");

            // DIALOGUE DEBUG ONLY, remove after done
            //doneWithLevel = true;
        }
    }

    // SPAWN ENEMIES
    private void SpawnRandomEnemies() {

        // TODO: If level number is the last level in area, then disregard everything except Boss spawning
        if (!IsArrayEmpty(bosses)) {
            bossLevel = true;
            Debug.Log(bosses);
        }

        // Miniboss spawning
        if (!IsArrayEmpty(minibosses) && !bossLevel) {

            // For every possible miniboss in the level (e.g. Scout, Chris)
            for (int m = 0; m < minibosses.Length; m++) {
                
                // Generate random max amount of each unique miniboss in level (e.g. 1 Scout, 1 Chris)
                int minibossesRange = UnityEngine.Random.Range(1, 2);
                
                // For the max amount of every different type of miniboss (e.g. for 1 Scout, for 1 Chris)
                for (int s = 0; s < minibossesRange; s++) {

                    // If able to get script—
                    if (minibosses[m].TryGetComponent<Enemy>(out var enemy) || minibosses[m].GetComponentInChildren<Enemy>()) {

                        if (enemy == null) {
                            enemy = minibosses[m].GetComponentInChildren<Enemy>();
                        }

                        // Current level
                        int level = GameStateManager.GetLevel();

                        if (minibossSpawned) {
                            level = 1;
                        }

                        // Roll to see if enemy is able to spawn
                        float spawnValue = UnityEngine.Random.value;
                        Debug.Log(spawnValue);

                        float exponent = Mathf.Pow(Mathf.Abs(enemy.spawnChanceMultiplier * level - enemy.spawnChanceXTransform), enemy.spawnChanceExponent);

                        // Complete spawn rate equation formula
                        float spawnChance = (1 / (enemy.spawnChanceVertAmp + exponent)) + enemy.spawnChanceYTransform;

                        // If enemy spawn roll is over the max chance, then compress to threshold
                        if (spawnChance > enemy.maxSpawnChance) {
                            spawnChance = enemy.maxSpawnChance;
                        }

                        Debug.Log(minibosses[m] + ": " + spawnChance);

                        // If enemy spawn roll is a success, spawn enemy 
                        if (spawnValue <= spawnChance) {

                            // Generates random number to pick Enemy spawnpoint
                            int randX = GetRandomXTile();
                            int randY = GetRandomTile();

                            // For as many floor tiles as there are in the tilemap:
                            for (int i = 0; i < tileListX.Count; i++) {
                                
                                // If suitable floor tiles have been found (Ground tiles and no obstacles on those tiles)
                                if (gridHandler[tileListX[randX], tileListY[randY]] == TileType.FLOOR) {

                                    Vector3 genPos = floorTilemap.CellToWorld(new Vector3Int(tileListX[randX], tileListY[randY]));

                                    if (genPos.x <= player.transform.position.x + spawnRadiusX 
                                    && genPos.x >= player.transform.position.x - spawnRadiusX
                                    && genPos.y <= player.transform.position.y + spawnRadiusY 
                                    && genPos.y >= player.transform.position.y - spawnRadiusY) {
                                        randX = GetRandomXTile();
                                        randY = GetRandomTile();
                                    } else {

                                        // Spawns Enemy
                                        if (minibosses[m].GetComponentInChildren<Enemy>()) {
                                            minibosses[m].GetComponentInChildren<Enemy>().Create(minibosses[m], new Vector2(tileListX[randX] * mapGrid.cellSize.x, tileListY[randY] * mapGrid.cellSize.y), Quaternion.identity, this);
                                            enemyTotal++;
                                            minibossSpawned = true;
                                            break;
                                        } else {
                                            enemy.Create(minibosses[m], new Vector2(tileListX[randX] * mapGrid.cellSize.x, tileListY[randY] * mapGrid.cellSize.y), Quaternion.identity, this);   
                                            enemyTotal++;
                                            minibossSpawned = true;
                                            break;
                                        }
                                    }

                                } else {
                                    
                                    // Generates random number to pick Enemy spawnpoint
                                    randX = GetRandomXTile();
                                    randY = GetRandomTile();
                                }
                            }
                        } else {
                            Debug.Log(minibosses[m] + " failed the spawn roll!");
                        }

                    } else {
                        Debug.Log("Could not find Enemy script component on this enemy!");
                    }
                }
            }
        }
    
        // Stationary enemy spawning
        if (!IsArrayEmpty(stationEnemies) && !bossLevel) {

            // For every common enemy in the level (e.g. Wispling, Slime, Joseph)
            for (int st = 0; st < stationEnemies.Length; st++) {

                // Generate random max amount of common enemies in level (e.g. 4 Wisplings, 5 Slimes, 1 Joseph)
                int stationRange = UnityEngine.Random.Range(1, 4);

                // For the max amount of every different type of stationary enemy (e.g. for 4 Wisplings, for 5 Slimes, for 1 Joseph)
                for (int s = 0; s < stationRange; s++) {

                    // If able to get script—
                    if (stationEnemies[st].TryGetComponent<Enemy>(out var enemy) || stationEnemies[st].GetComponentInChildren<Enemy>()) {

                        if (enemy == null) {
                            enemy = stationEnemies[st].GetComponentInChildren<Enemy>();
                        }

                        // Current level
                        int level = GameStateManager.GetLevel();

                        // Roll to see if enemy is able to spawn
                        var spawnValue = UnityEngine.Random.value;
                        Debug.Log(spawnValue);

                        var exponent = Mathf.Pow(Mathf.Abs(enemy.spawnChanceMultiplier * level - enemy.spawnChanceXTransform), enemy.spawnChanceExponent);

                        // Complete spawn rate equation formula
                        var spawnChance = (1 / (enemy.spawnChanceVertAmp + exponent)) + enemy.spawnChanceYTransform;

                        // If enemy spawn roll is over the max chance, then compress to threshold
                        if (spawnChance > enemy.maxSpawnChance) {
                            spawnChance = enemy.maxSpawnChance;
                        }

                        Debug.Log(stationEnemies[st] + ": " + spawnChance);

                        // If enemy spawn roll is a success, spawn enemy 
                        if (spawnValue <= spawnChance) {

                            // Generates random number to pick Enemy spawnpoint
                            int randX = GetRandomXTile();
                            int randY = GetRandomTile();

                            // For as many floor tiles as there are in the tilemap:
                            for (int i = 0; i < tileListX.Count; i++) {

                                // Choose from the available floor tiles
                                if (gridHandler[tileListX[randX], tileListY[randY]] == TileType.WALLS) {

                                    Vector3 genPos = floorTilemap.CellToWorld(new Vector3Int(tileListX[randX], tileListY[randY]));

                                    if (genPos.x <= player.transform.position.x + (spawnRadiusX/2) 
                                    && genPos.x >= player.transform.position.x - (spawnRadiusX/2)
                                    && genPos.y <= player.transform.position.y + (spawnRadiusY/2) 
                                    && genPos.y >= player.transform.position.y - (spawnRadiusY/2)) {
                                        randX = GetRandomXTile();
                                        randY = GetRandomTile();
                                    } else {

                                        Quaternion rot = Quaternion.Euler(0, 0, 0);

                                        // TOP WALL
                                        if (gridHandler[tileListX[randX], tileListY[randY] - 1] == TileType.FLOOR) {
                                            rot = Quaternion.Euler(0, 0, -90);

                                            float xCoord = (tileListX[randX] + 0.5f) * mapGrid.cellSize.x;
                                            float yCoord = (tileListY[randY] - 0.3f) * mapGrid.cellSize.y;
                                            Debug.Log("TOP WALL SPAWNED");

                                            // Spawns Enemy
                                            if (stationEnemies[st].GetComponentInChildren<Enemy>()) {
                                                stationEnemies[st].GetComponentInChildren<Enemy>().Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);
                                                enemyTotal++;
                                                break;
                                            } else {
                                                gridHandler[tileListX[randX], tileListY[randY]] = TileType.BORDER;
                                                enemy.Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);   
                                                enemyTotal++;
                                                break;
                                            }
                                        } 
                                        // BOTTOM WALL
                                        else if (gridHandler[tileListX[randX], tileListY[randY] + 1] == TileType.FLOOR) {
                                            rot = Quaternion.Euler(0, 0, 90);

                                            float xCoord = (tileListX[randX] + 0.5f) * mapGrid.cellSize.x;
                                            float yCoord = (tileListY[randY] + 1) * mapGrid.cellSize.y;

                                            Debug.Log("BOTTOM WALL SPAWNED");
                                            // Spawns Enemy
                                            if (stationEnemies[st].GetComponentInChildren<Enemy>()) {
                                                stationEnemies[st].GetComponentInChildren<Enemy>().Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);
                                                enemyTotal++;
                                                break;
                                            } else {
                                                gridHandler[tileListX[randX], tileListY[randY]] = TileType.BORDER;
                                                enemy.Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);   
                                                enemyTotal++;
                                                break;
                                            }
                                        } 
                                        // LEFT WALL
                                        else if (gridHandler[tileListX[randX] + 1, tileListY[randY]] == TileType.FLOOR) {
                                            rot = Quaternion.Euler(0, 0, 0);

                                            float xCoord = (tileListX[randX] + 1f) * mapGrid.cellSize.x;
                                            float yCoord = (tileListY[randY] + 0.6f) * mapGrid.cellSize.y;

                                            Debug.Log("LEFT WALL SPAWNED");
                                            // Spawns Enemy
                                            if (stationEnemies[st].GetComponentInChildren<Enemy>()) {
                                                stationEnemies[st].GetComponentInChildren<Enemy>().Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);
                                                enemyTotal++;
                                                break;
                                            } else {
                                                gridHandler[tileListX[randX], tileListY[randY]] = TileType.BORDER;
                                                enemy.Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);   
                                                enemyTotal++;
                                                break;
                                            }
                                        } 
                                        // RIGHT WALL
                                        else if (gridHandler[tileListX[randX] - 1, tileListY[randY]] == TileType.FLOOR) {
                                            rot = Quaternion.Euler(0, 0, 180);

                                            float xCoord = (tileListX[randX] * mapGrid.cellSize.x);
                                            float yCoord = (tileListY[randY] + 0.3f) * mapGrid.cellSize.y;

                                            Debug.Log("RIGHT WALL SPAWNED");
                                            // Spawns Enemy
                                            if (stationEnemies[st].GetComponentInChildren<Enemy>()) {
                                                stationEnemies[st].GetComponentInChildren<Enemy>().Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);
                                                enemyTotal++;
                                                break;
                                            } else {
                                                gridHandler[tileListX[randX], tileListY[randY]] = TileType.BORDER;
                                                enemy.Create(stationEnemies[st], new Vector2(xCoord, yCoord), rot, this);   
                                                enemyTotal++;
                                                break;
                                            }
                                        } 
                                        else {
                                            // Generates random number to pick Enemy spawnpoint
                                            randX = GetRandomXTile();
                                            randY = GetRandomTile();
                                        }
                                    }

                                } else {
                                    
                                    // Generates random number to pick Enemy spawnpoint
                                    randX = GetRandomXTile();
                                    randY = GetRandomTile();
                                }
                            }

                        } else {
                            Debug.Log(stationEnemies[st] + " failed the spawn roll!");
                        }

                    } else {
                        Debug.Log("Could not find Enemy script component on this enemy!");
                    }
                }
            }
        }
        
        // Rare enemy spawning
        if (!IsArrayEmpty(rareEnemies) && !bossLevel) {
            
            // For every rare enemy in the level (e.g. Deforestation Guy, Nancy)
            for (int r = 0; r < rareEnemies.Length; r++) {
                
                // Generate random amount of rare enemies in level (e.g. 3 Deforestation Guy, 2 Nancy)
                int rareRange = UnityEngine.Random.Range(1, 3);
                
                // For the amount of every different type of rare enemy (e.g. for 3 Deforestation Guy, for 2 Nancy)
                for (int s = 0; s < rareRange; s++) {

                    // If able to get script—
                    if (rareEnemies[r].TryGetComponent<Enemy>(out var enemy) || rareEnemies[r].GetComponentInChildren<Enemy>()) {

                        if (enemy == null) {
                            enemy = rareEnemies[r].GetComponentInChildren<Enemy>();
                        }

                        // Current level
                        int level = GameStateManager.GetLevel();

                        // Roll to see if enemy is able to spawn
                        float spawnValue = UnityEngine.Random.value;

                        float exponent = Mathf.Pow(Mathf.Abs(enemy.spawnChanceMultiplier * level - enemy.spawnChanceXTransform), enemy.spawnChanceExponent);

                        // Complete spawn rate equation formula
                        float spawnChance = (1 / (enemy.spawnChanceVertAmp + exponent)) + enemy.spawnChanceYTransform;

                        // If enemy spawn roll is over the max chance, then compress to threshold
                        if (spawnChance > enemy.maxSpawnChance) {
                            spawnChance = enemy.maxSpawnChance;
                        }

                        Debug.Log(rareEnemies[r] + ": " + spawnChance);

                        // If enemy spawn roll is a success, spawn enemy 
                        if (spawnValue <= spawnChance) {

                            // Generates random number to pick Enemy spawnpoint
                            int rand = GetRandomTile();

                            // For as many floor tiles as there are in the tilemap:
                            for (int i = 0; i < tileListX.Count; i++) {

                                // If suitable floor tiles have been found (Ground tiles and no obstacles on those tiles)
                                if (gridHandler[tileListX[rand], tileListY[rand]] == TileType.FLOOR) {

                                    Vector3 genPos = floorTilemap.CellToWorld(new Vector3Int(tileListX[rand], tileListY[rand]));

                                    if (genPos.x <= player.transform.position.x + spawnRadiusX 
                                    && genPos.x >= player.transform.position.x - spawnRadiusX
                                    && genPos.y <= player.transform.position.y + spawnRadiusY 
                                    && genPos.y >= player.transform.position.y - spawnRadiusY) {
                                        rand = GetRandomTile();
                                    } else {

                                        if (rareEnemies[r].GetComponentInChildren<Enemy>()) {
                                            rareEnemies[r].GetComponentInChildren<Enemy>().Create(rareEnemies[r], new Vector2(tileListX[rand] * mapGrid.cellSize.x, tileListY[rand] * mapGrid.cellSize.y), Quaternion.identity, this);
                                            enemyTotal++;
                                            break;
                                        } else {
                                            enemy.Create(rareEnemies[r], new Vector2(tileListX[rand] * mapGrid.cellSize.x, tileListY[rand] * mapGrid.cellSize.y), Quaternion.identity, this);   
                                            enemyTotal++;
                                            break;
                                        }
                                    }

                                } else {
                                    
                                    // Generates random number to pick Enemy spawnpoint
                                    rand = GetRandomTile();
                                }
                            }

                        } else {
                            Debug.Log(rareEnemies[r] + " failed the spawn roll!");
                        }

                    } else {
                        Debug.Log("Could not find Enemy script component on this enemy!");
                    }
                }
            }
        }
        
        // Common enemy spawning
        if (!IsArrayEmpty(commonEnemies) && !bossLevel) {
            // For every common enemy in the level (e.g. Wispling, Slime, Joseph)
            for (int c = 0; c < commonEnemies.Length; c++) {

                // Generate random amount of common enemies in level (e.g. 4 Wisplings, 5 Slimes, 1 Joseph)
                int commonRange = UnityEngine.Random.Range(3, 5);

                // For the amount of every different type of common enemy (e.g. for 4 Wisplings, for 5 Slimes, for 1 Joseph)
                for (int s = 0; s < commonRange; s++) {

                    // If able to get script—
                    if (commonEnemies[c].TryGetComponent<Enemy>(out var enemy) || commonEnemies[c].GetComponentInChildren<Enemy>()) {

                        if (enemy == null) {
                            enemy = commonEnemies[c].GetComponentInChildren<Enemy>();
                        }

                        // Current level
                        int level = GameStateManager.GetLevel();

                        // Roll to see if enemy is able to spawn
                        float spawnValue = UnityEngine.Random.value;

                        float exponent = Mathf.Pow(Mathf.Abs(enemy.spawnChanceMultiplier * level - enemy.spawnChanceXTransform), enemy.spawnChanceExponent);

                        // Complete spawn rate equation formula
                        float spawnChance = (1 / (enemy.spawnChanceVertAmp + exponent)) + enemy.spawnChanceYTransform;

                        // If enemy spawn roll is over the max chance, then compress to threshold
                        if (spawnChance > enemy.maxSpawnChance) {
                            spawnChance = enemy.maxSpawnChance;
                        }

                        Debug.Log(commonEnemies[c] + ": " + spawnChance);

                        // If enemy spawn roll is a success, spawn enemy 
                        if (spawnValue <= spawnChance) {

                            // Generates random number to pick Enemy spawnpoint
                            int rand = GetRandomTile();

                            // For as many floor tiles as there are in the tilemap:
                            for (int i = 0; i < tileListX.Count; i++) {

                                // If suitable floor tiles have been found (Ground tiles and no obstacles on those tiles)
                                if (gridHandler[tileListX[rand], tileListY[rand]] == TileType.FLOOR) {

                                    Vector3 genPos = floorTilemap.CellToWorld(new Vector3Int(tileListX[rand], tileListY[rand]));

                                    if (genPos.x <= player.transform.position.x + spawnRadiusX 
                                    && genPos.x >= player.transform.position.x - spawnRadiusX
                                    && genPos.y <= player.transform.position.y + spawnRadiusY 
                                    && genPos.y >= player.transform.position.y - spawnRadiusY) {
                                        rand = GetRandomTile();
                                    } else {
                                        
                                        // Spawns Enemy
                                        if (commonEnemies[c].GetComponentInChildren<Enemy>()) {
                                            commonEnemies[c].GetComponentInChildren<Enemy>().Create(commonEnemies[c], new Vector2(tileListX[rand] * mapGrid.cellSize.x, tileListY[rand] * mapGrid.cellSize.y), Quaternion.identity, this);
                                            enemyTotal++;
                                            break;
                                        } else {
                                            enemy.Create(commonEnemies[c], new Vector2(tileListX[rand] * mapGrid.cellSize.x, tileListY[rand] * mapGrid.cellSize.y), Quaternion.identity, this);   
                                            enemyTotal++;
                                            break;
                                        }
                                    }
                                } else {
                                    
                                    // Generates random number to pick Enemy spawnpoint
                                    rand = GetRandomTile();
                                }
                            }
                            
                        } else {
                            Debug.Log(commonEnemies[c] + " failed the spawn roll!");
                        }

                    } else {
                        Debug.Log("Could not find Enemy script component on this enemy!");
                    }
                }
            }
        }

        Debug.Log("Spawned Enemies!");
    }

    private bool IsArrayEmpty(GameObject[] array) {
        return Array.TrueForAll(array, x => x == null);
    }

    public int GetRandomGridXTile() {
        return UnityEngine.Random.Range(0, gridHandler.GetLength(0) - 1);
    }

    public int GetRandomGridYTile() {
        return UnityEngine.Random.Range(0, gridHandler.GetLength(1) - 1);
    }

    public int GetRandomTile() {
        return UnityEngine.Random.Range(0, tileListY.Count);
     }

    public int GetRandomXTile() {
        return UnityEngine.Random.Range(0, tileListX.Count);
    }

    public bool CheckGroundTile(Vector3 vector) {
        return floorTilemap.GetSprite(new Vector3Int((int)vector.x, (int)vector.y, 0)) == tiles.ground;
    }

    // GENERATE PATHFINDING MAP
    public void PathScan() {
        AstarPath.FindAstarPath();

        AstarData aData = AstarPath.active.data;
        GridGraph gg = aData.gridGraph;

        // Gets the distance from 0,0 on the x axis (half of the total tilemap size)
        float radiusX = (mapGrid.cellSize.x * (mapWidth - 1f)) / 2f;
        // Gets the distance from 0,0 on the y axis (half of the total tilemap size)
        float radiusY = (mapGrid.cellSize.y * (mapHeight - 1f)) / 2f;

        gg.SetDimensions((mapWidth - 1) * 4, (mapHeight - 1) * 4, mapGrid.cellSize.x / 4f);
        gg.center = new Vector3(radiusX, radiusY, 0);

        if (AstarPath.active.data.gridGraph.nodes == null) {
            //Physics2D.SyncTransforms();
            AstarPath.active.Scan();
        }
        if (AstarPath.active.data.gridGraph.nodes != null) {
            AstarPath.active.UpdateGraphs(wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
        }

        Debug.Log("Pathfinding map scanned!");
    }

    public void SaveMap () {
        SaveSystem.SaveMap(this);
    }

    public void LoadMap () {

        // Load save data
        MapData data = SaveSystem.LoadMap();

        GameStateManager.SetLevel(data.levelNum);
        GameStateManager.SetStage(data.stageNum);

        // Clear tile locations
        tileListX.Clear();
        tileListY.Clear();
        tileCount = 0;

        int listNum = 0;

        // For every tile slot in our grid
        for (int x = 0; x < gridHandler.GetLength(0); x++) {

            for (int y = 0; y < gridHandler.GetLength(1); y++) {

                // Add tile x and y locations
                tileListX.Add(x);
                tileListY.Add(y);

                // If save data indicates 1, then set current tile position / type to a FLOOR tile
                if (data.tileTypes[listNum] == 1) {
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), tiles.floor);
                    gridHandler[x, y] = TileType.FLOOR;
                    tileCount++;
                }
                /* If save data indicates 2, then set current tile position / type to an EMPTY tile
                else if (data.tileTypes[listNum] == 2) {

                    floorTilemap.SetTile(new Vector3Int(x, y, 0), tiles.empty);
                    gridHandler[x, y] = Grid.EMPTY;
                    tileCount++;
                }*/
                // If save data indicates 3, then set current tile position / type to an EMPTY tile (for wall generation)
                else if (data.tileTypes[listNum] == 2) {
                    //floorTilemap.SetTile(new Vector3Int(x, y, 0), tiles.empty);
                    gridHandler[x, y] = TileType.EMPTY;
                    //tileCount++;
                }

                // If save data indicates 1 in obstacle tile lists, then set current tile position / type to an OBSTACLE tile
                if (data.oTileTypes[listNum] == 1) {
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), tiles.floor);
                    oTilemap.SetTile(new Vector3Int(x, y, 0), tiles.obstacles);
                    gridHandler[x, y] = TileType.OBSTACLES;
                    oTileCount++;
                    oTileCount++;
                }

                // Increment tile number
                listNum++;
                //tileCount++;
            }
        }

        // Set grid origin to empty
        floorTilemap.SetTile(new Vector3Int(0, 0, 0), tiles.empty);
        tileCount--;
        gridHandler[0, 0] = TileType.EMPTY;

        // Create walls around map
        CreateWalls();
        FillFloors();
        CreateBorders();
        /* if (!bossLevel) {
            CreateObstacles();
        } */
        CreateBreakables();
    }

}

// TILE TYPES
public enum TileType {
    FLOOR, EMPTY, OBSTACLES, BORDER, WALLS
}