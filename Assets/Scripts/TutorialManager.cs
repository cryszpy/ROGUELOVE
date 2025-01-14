using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] private WalkerGenerator gen;

    [SerializeField] private GameObject npcSpawn1;
    [SerializeField] private GameObject weaponSpawn1;
    [SerializeField] private GameObject enemySpawn1;
    [SerializeField] private GameObject enemyCameraFocus;
    [SerializeField] private List<GameObject> enemySpawnList;

    [SerializeField] private GameObject chestSpawn;
    [SerializeField] private GameObject npcSpawn2;
    [SerializeField] private GameObject bossSpawn;

    [SerializeField] private GameObject weapon;

    private CinemachineCamera cam;
    [SerializeField] private PixelPerfectCamera camPixelPerfect;
    [SerializeField] private CinemachinePixelPerfect cinemachinePixelPerfect;
    
    public Transform cameraLookAt;

    [SerializeField] private GameObject enemyFake;
    [SerializeField] private GameObject enemyReal;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject npcToSpawn;
    [SerializeField] private GameObject chestToSpawn;

    public Rigidbody2D npcRb;
    private GameObject spawnedEnemy;

    [SerializeField] private int tutorialStage;

    private Animator letterboxAnimator;

    [SerializeField] private List<DialoguePiece> totalDialogue = new();

    private Queue<DialoguePiece> dialogueQueue;

    private bool moveNPC;
    private float moveTimer = 0;

    public int enemiesKilled = 0;

    private void OnEnable() {
        GameStateManager.EOnDialogueEnd += ContinueTutorial;
    }

    private void OnDisable() {
        DisconnectEvents();
    }

    private void DisconnectEvents() {
        GameStateManager.EOnDialogueEnd -= ContinueTutorial;
        PlayerController.EOnWeaponPickup -= ContinueTutorial;
        GameStateManager.EOnEnemyDeath -= ContinueTutorial;
        GameStateManager.EOnWeaponSwitch -= ContinueTutorial;
        GameStateManager.EOnWeaponDrop -= ContinueTutorial;
        GameStateManager.EOnEnemyDeath -= CheckEnemyStatus;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        dialogueQueue = new(totalDialogue);

        gen.playerCont.PlayerStart(false);
        gen.playerCont.ResetRun();
        gen.playerCont.ableToDrop = false;

        tutorialStage = 0;

        if (!cam) {
            cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineCamera>();
        }
        if (!letterboxAnimator) {
            letterboxAnimator = GameObject.FindGameObjectWithTag("Letterbox").GetComponent<Animator>();
        }

        StartCoroutine(InitialCameraSway());
    }

    public void ContinueTutorial() {
        GameStateManager.SetState(GAMESTATE.MENU);

        switch (tutorialStage) {
            case 1:
                Debug.Log("ContinueTutorial 1");

                PlayerController.EOnWeaponPickup += ContinueTutorial;

                // Switch back from dialogue to show weapon spawned
                WeaponSway();
                break;
            case 2:
                Debug.Log("ContinueTutorial 2 - PLAYER HAS PICKED UP WEAPON");

                // PLAYER HAS PICKED UP WEAPON!

                PlayerController.EOnWeaponPickup -= ContinueTutorial;

                // Switch back to NPC focus after picking up weapon
                StartCoroutine(CameraSway());
                break;
            case 3:
                Debug.Log("ContinueTutorial 3");

                // Switch to show enemy spawning
                StartCoroutine(CameraSway());
                break;
            case 4:
                Debug.Log("ContinueTutorial 4 - PLAYER HAS KILLED ENEMY");

                // PLAYER HAS KILLED ENEMY!

                GameStateManager.EOnEnemyDeath -= ContinueTutorial;

                // Switch back to NPC focus after killing enemy
                StartCoroutine(CameraSway());
                break;
            case 5:
                Debug.Log("ContinueTutorial 5");

                // Go to stage 5 of tutorial
                StartCoroutine(Tutorial());
                break;
            case 6:
                Debug.Log("ContinueTutorial 6 - PLAYER HAS HIT TRIGGER");

                // Switch to chest spawn
                ChestSway();

                StartCoroutine(Tutorial());
                break;
            case 7:
                Debug.Log("ContinueTutorial 7");

                StartCoroutine(Tutorial());
                break;
            case 8:
                Debug.Log("ContinueTutorial 8 - PLAYER HAS PICKED UP WEAPON");

                PlayerController.EOnWeaponPickup -= ContinueTutorial;

                // Switch back to NPC focus after picking up chest weapon
                StartCoroutine(CameraSway());
                break;
            case 9:
                Debug.Log("ContinueTutorial 9");

                // Stage 9
                StartCoroutine(Tutorial());
                break;
            case 10:
                Debug.Log("ContinueTutorial 10 - PLAYER HAS SWITCHED WEAPON");

                GameStateManager.EOnWeaponSwitch -= ContinueTutorial;

                // Switch to NPC after switching weapon
                StartCoroutine(CameraSway());
                break;
            case 11:
                Debug.Log("ContinueTutorial 11");

                StartCoroutine(Tutorial());
                break;
            case 12:
                Debug.Log("ContinueTutorial 12 - PLAYER HAS DROPPED WEAPON");

                GameStateManager.EOnWeaponDrop -= ContinueTutorial;

                StartCoroutine(CameraSway());
                break;
            case 13:
                Debug.Log("ContinueTutorial 13");

                StartCoroutine(EnemySpawnSway());
                break;
            case 14:
                Debug.Log("ContinueTutorial 14 - ALL ENEMIES DEAD");

                GameStateManager.EOnEnemyDeath -= CheckEnemyStatus;

                // Move camera to boss point 2
                StartCoroutine(CameraSway());
                break;
            case 15:
                Debug.Log("ContinueTutorial 15");

                // Despawn NPC, move camera to boss spawn and spawn boss
                StartCoroutine(Tutorial());

                break;
            case 16:
                Debug.Log("ContinueTutorial 16 - BOSS DEFEATED AND DOORWAY SPAWNED");

                GameStateManager.EOnEnemyDeath -= ContinueTutorial;

                // Swap camera to defeated boss NPC
                StartCoroutine(CameraSway());

                break;
            case 17:
                Debug.Log("ContinueTutorial 17");

                GameStateManager.EOnDoorwaySpawn -= ContinueTutorial;

                StartCoroutine(Tutorial());

                break;
        }
    }

    private IEnumerator Tutorial() {

        switch (tutorialStage) {
            case 0:
                Debug.Log("0");

                // Cue letterbox hide animation
                LetterboxAnimation(true);

                // Spawn NPC
                SpawnTutorialNPC(npcSpawn1);

                yield return new WaitForSeconds(1f);

                tutorialStage++;

                // Start dialogue
                DialoguePiece pickupWeapon = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(pickupWeapon, false);
                
                break; 
            case 1:
                Debug.Log("1");

                yield return new WaitForSeconds(0.5f);

                // Spawn weapon
                if (weapon) {
                    SpawnPickup(weapon, weaponSpawn1);
                }

                yield return new WaitForSeconds(1f);

                tutorialStage++;

                // Switch camera back to player
                ReturnSway();

                break;

            case 2:
                Debug.Log("2");
                tutorialStage++;
                
                // Start dialogue
                DialoguePiece fireWeapon = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(fireWeapon, false);
                
                break;

            case 3:
                Debug.Log("3");

                // Spawn enemy
                SpawnEnemy(enemyFake, enemySpawn1);

                // Add requirement of killing enemy
                GameStateManager.EOnEnemyDeath += ContinueTutorial;

                yield return new WaitForSeconds(2f);

                tutorialStage++;

                ReturnSway();
                
                break;

            case 4:
                Debug.Log("4");

                tutorialStage++;

                // Start dialogue
                DialoguePiece enemyKilled = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(enemyKilled, false);

                break;
            case 5:
                Debug.Log("5");

                // Breaks walls
                RemoveWalls();

                yield return new WaitForSeconds(0.25f);

                // NPC moves away
                moveNPC = true;

                yield return new WaitForSeconds(2f);

                tutorialStage++;

                ReturnSway();
                
                break;
            case 6:
                Debug.Log("6");

                // Spawn NPC at second spawn point
                SpawnTutorialNPC(npcSpawn2);

                yield return new WaitForSeconds(0.5f);

                // Spawn chest
                SpawnChest(chestSpawn);

                yield return new WaitForSeconds(1.5f);

                cam.Follow = npcSpawn2.transform;

                tutorialStage++;

                // Start dialogue about chests
                DialoguePiece chestSpawned = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(chestSpawned, false);

                break;
            case 7:
                Debug.Log("7");

                // Adds requirement of picking up weapon
                PlayerController.EOnWeaponPickup += ContinueTutorial;

                tutorialStage++;

                // Return to player and wait for chest weapon pickup
                ReturnSway();
                
                break;
            case 8:
                Debug.Log("8");

                tutorialStage++;

                // Start dialogue about weapon switching
                DialoguePiece chestWeaponPickup = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(chestWeaponPickup, false);

                break;
            case 9:
                Debug.Log("9");

                // Adds requirement of switching weapon
                GameStateManager.EOnWeaponSwitch += ContinueTutorial;

                tutorialStage++;

                // Return to player and wait for weapon switch
                ReturnSway();

                break;
            case 10:
                Debug.Log("10");

                tutorialStage++;

                // Start dialogue about weapon dropping
                DialoguePiece weaponDropping = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(weaponDropping, false);

                break;
            case 11:
                Debug.Log("11");

                gen.playerCont.ableToDrop = true;

                // Adds requirement of dropping weapon
                GameStateManager.EOnWeaponDrop += ContinueTutorial;

                tutorialStage++;

                // Return to player and wait for weapon drop
                ReturnSway();

                break;
            case 12:
                Debug.Log("12");

                tutorialStage++;

                // Start dialogue about big enemy fight
                DialoguePiece enemyFight = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(enemyFight, false);

                break;
            case 13:
                Debug.Log("13");

                tutorialStage++;

                // Adds requirement of all enemies dead
                GameStateManager.EOnEnemyDeath += CheckEnemyStatus;

                GameStateManager.SetState(GAMESTATE.PLAYING);

                break;
            case 14:
                Debug.Log("14");

                tutorialStage++;

                // Start dialogue preceding boss fight
                DialoguePiece bossFight = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(bossFight, false);

                break;
            case 15:
                Debug.Log("15");

                yield return new WaitForSeconds(0.35f);

                // Despawn NPC, move camera to boss spawn and spawn boss
                Destroy(npcRb.gameObject);

                yield return new WaitForSeconds(1f);

                // Move camera to boss spawn
                cam.Follow = bossSpawn.transform;

                yield return new WaitForSeconds(1f);

                // Spawn boss
                SpawnEnemy(boss, bossSpawn);
                WalkerGenerator.SetDeadEnemy(0);
                WalkerGenerator.EnemyTotal = 0;
                WalkerGenerator.EnemyTotal++;

                // Add requirement of doorway spawning
                GameStateManager.EOnDoorwaySpawn += ContinueTutorial;

                yield return new WaitForSeconds(2f);

                tutorialStage++;

                ReturnSway();

                break;
            case 16:
                Debug.Log("16");

                yield return new WaitForSeconds(0.75f);

                tutorialStage++;

                // Start dialogue post-bossfight
                DialoguePiece bossDefeated = dialogueQueue.Dequeue();
                GameStateManager.dialogueManager.StartDialogue(bossDefeated, false);

                break;
            case 17:
                Debug.Log("17");

                // Removes all functions linked to events
                DisconnectEvents();

                // Tutorial done
                HomeManager.TutorialDone = true;
                GameStateManager.homeManager.SaveHome();

                // Return camera focus to player
                ReturnSway();

                break;
        }
    }

    private void FixedUpdate() {
        
        if (moveNPC) {
            moveTimer += Time.fixedDeltaTime;
            npcRb.AddForce(1800 * Time.fixedDeltaTime * new Vector2(1, 0));

            if (moveTimer > 4f) {
                moveNPC = false;
                moveTimer = 0;
                Destroy(npcRb.gameObject);
            }
        }
    }

    private void RemoveWalls() {
        GameStateManager.SetState(GAMESTATE.MENU);

        gen.gridHandler[24, 6] = TileType.EMPTY;
        gen.wallsTilemap.SetTile(new Vector3Int(24, 6), gen.tiles.empty);

        gen.gridHandler[25, 6] = TileType.EMPTY;
        gen.wallsTilemap.SetTile(new Vector3Int(25, 6), gen.tiles.empty);

        AstarPath.active.UpdateGraphs(gen.wallsTilemap.gameObject.GetComponent<TilemapCollider2D>().bounds);
    }

    private void LetterboxAnimation(bool value) {
        GameStateManager.SetState(GAMESTATE.MENU);

        // Set manual control boolean
        TransitionManager.letterboxManualControl = value;

        // Show/Hide other UI elements
        GameStateManager.dialogueManager.UIElementsAnimation(value);

        // Show/Hide letterbox
        letterboxAnimator.SetBool("Show", value);
    }

    private IEnumerator InitialCameraSway() {
        GameStateManager.SetState(GAMESTATE.MENU);

        yield return new WaitForSeconds(0.5f);

        // Cue letterbox animation
        LetterboxAnimation(true);

        yield return new WaitForSeconds(1.5f);

        cam.Follow = npcSpawn1.transform;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Tutorial());
        
        /* cam.Follow = cameraLookAt;
        WalkerGenerator.doneWithLevel = true;
        GameStateManager.SetState(gameState); */
    }

    private IEnumerator CameraSway() {
        GameStateManager.SetState(GAMESTATE.MENU);

        // Cue letterbox animation
        LetterboxAnimation(true);

        yield return new WaitForSeconds(0.5f);

        if (tutorialStage == 3) {
            cam.Follow = enemySpawn1.transform;
        } else if (tutorialStage == 16) {
            cam.Follow = spawnedEnemy.transform;
        }
        else if (tutorialStage > 7) {
            cam.Follow = npcSpawn2.transform;
        }
        else {
            cam.Follow = npcSpawn1.transform;
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(Tutorial());
        
        /* cam.Follow = cameraLookAt;
        WalkerGenerator.doneWithLevel = true;
        GameStateManager.SetState(gameState); */
    }

    private void ReturnSway() {

        // Cue letterbox hide animation
        LetterboxAnimation(false);

        cam.Follow = cameraLookAt.transform;
        GameStateManager.SetState(GAMESTATE.PLAYING);
    }

    private void WeaponSway() {
        GameStateManager.SetState(GAMESTATE.MENU);

        cam.Follow = weaponSpawn1.transform;

        StartCoroutine(Tutorial());
    }

    private void ChestSway() {
        GameStateManager.SetState(GAMESTATE.MENU);

        // Cue letterbox animation
        LetterboxAnimation(true);

        cam.Follow = chestSpawn.transform;
    }

    private IEnumerator EnemySpawnSway() {
        GameStateManager.SetState(GAMESTATE.MENU);

        cam.Follow = enemyCameraFocus.transform;
        
        camPixelPerfect.enabled = false;
        cinemachinePixelPerfect.enabled = false;

        cam.Lens.OrthographicSize = 3.2f;

        // Zoom out
        while (cam.Lens.OrthographicSize < 9) {
            cam.Lens.OrthographicSize += 0.1f;

            yield return new WaitForSeconds(0.005f);
        }

        foreach (var spawn in enemySpawnList) {
            SpawnEnemy(enemyReal, spawn);
        }

        yield return new WaitForSeconds(1.5f);

        ReturnSway();

        while (cam.Lens.OrthographicSize > 3.2f) {
            cam.Lens.OrthographicSize -= 0.1f;

            yield return new WaitForSeconds(0.005f);
        }

        camPixelPerfect.enabled = true;
        cinemachinePixelPerfect.enabled = true;

        StartCoroutine(Tutorial());
    }

    private void CheckEnemyStatus() {
        enemiesKilled++;

        if (enemiesKilled >= 7) {
            ContinueTutorial();
        }
    }

    private void SpawnEnemy(GameObject entity, GameObject spawnPoint) {
        
        if (entity) {

            // If able to get script—
            if (entity.TryGetComponent<Enemy>(out var enemy) || entity.GetComponentInChildren<Enemy>()) {

                if (enemy == null) {
                    enemy = entity.GetComponentInChildren<Enemy>();
                }

                // Spawns Enemy
                GameObject spawned = (GameObject)enemy.Create(entity, spawnPoint.transform.position, Quaternion.identity, gen);  
                spawnedEnemy = spawned; 
            }
        }
    }

    private void SpawnTutorialNPC(GameObject spawnPoint) {

        if (npcToSpawn) {

            GameObject npc = Instantiate(npcToSpawn, spawnPoint.transform.position, Quaternion.identity);

            npcRb = npc.GetComponent<Rigidbody2D>();
        }
    }

    private void SpawnChest(GameObject spawnPoint) {
        
        if (chestToSpawn) {

            GameObject chest = Instantiate(chestToSpawn, spawnPoint.transform.position, Quaternion.identity);
        }
    }

    private void SpawnPickup(GameObject pickup, GameObject spawnPoint) {

        GameObject spawned = Instantiate(pickup, spawnPoint.transform.position, Quaternion.identity);

    }
}
