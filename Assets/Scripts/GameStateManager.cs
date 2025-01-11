using UnityEngine;

// State of the game
public enum GAMESTATE {
    MAINMENU, MENU, PLAYING, PAUSED, GAMEOVER
}

public class GameStateManager : MonoBehaviour
{
    public static string PreviousScene { get; private set; }

    // Reference to the active instance of this object
    private static GameStateManager instance;

    // Dialogue Manager attached to this GameObject
    public static DialogueManager dialogueManager;

    public static PickupManager pickupManager;

    public static TransitionManager transitionManager;

    public static HomeManager homeManager;

    public static SaveType currentSaveType;

    public static bool tutorialEnabled;

    // Methods to check the state of the game
    private static GAMESTATE state;
    public static GAMESTATE GetState() {
        return state;
    }
    public static void SetState(GAMESTATE newState) {
        state = newState;
        Time.timeScale = state switch
        {
            GAMESTATE.MAINMENU => 1,
            GAMESTATE.MENU => 1,
            GAMESTATE.GAMEOVER => 1,
            GAMESTATE.PAUSED => 0,
            GAMESTATE.PLAYING => 1,
            _ => (float)1,
        };

        Cursor.visible = newState switch
        {
            GAMESTATE.MAINMENU => true,
            GAMESTATE.MENU => true,
            GAMESTATE.PLAYING => false,
            GAMESTATE.PAUSED => true,
            GAMESTATE.GAMEOVER => false,
            _ => false,
        };

        EOnGamestateChange?.Invoke();
    }

    // Methods to check whether the load save button was pressed
    private static bool saveState;
    public static void SetSave(bool condition) {
        saveState = condition;
    }
    public static bool SavePressed() {
        return saveState;
    }

    // Methods to check the current area level
    private static int currentLevel;
    public static void SetLevel(int level) {
        currentLevel = level;
    }
    public static void IncrementLevel(int level) {
        currentLevel += level;
    }
    public static int GetLevel() {
        return currentLevel;
    }

    // Methods to check the current stage
    private static int currentStage;
    public static void SetStage(int stage) {
        currentStage = stage;
    }
    public static int GetStage() {
        return currentStage;
    }

    public int stageTracker;

    public static bool levelClear = false;
    public static void SetLevelClear(bool condition) {
        levelClear = condition;
    }
    public static bool GetLevelClear() {
        return levelClear;
    }

    public GAMESTATE gameStateTracker;
    public SaveType saveTypeTracker;

    public delegate void EventHandler();
    public static EventHandler EOnTutorialTrigger;

    public static EventHandler EOnDialogueEnd;
    public static EventHandler EOnEnemyDeath;
    public static EventHandler EOnGamestateChange;
    public static EventHandler EOnWeaponDrop;
    public static EventHandler EOnWeaponSwitch;
    public static EventHandler EOnDoorwaySpawn;

    void Awake() {

        if (instance != null) {
            pickupManager = GameObject.FindGameObjectWithTag("PickupManager").GetComponent<PickupManager>();
            Destroy(gameObject);
        } else {
            instance = this;
            dialogueManager = GetComponent<DialogueManager>();
            transitionManager = GameObject.FindGameObjectWithTag("TransitionManager").GetComponent<TransitionManager>();
            homeManager = GetComponent<HomeManager>();
            DontDestroyOnLoad(gameObject);
        }

        if (GetStage() != 0) {
            SetState(GAMESTATE.PLAYING);
        }

        //Debug.Log("previous scene: " + GameStateManager.PreviousScene);  // use this in any level to get the last level.
    }

    void Update() {
        stageTracker = GetStage();
        gameStateTracker = GetState();
        saveTypeTracker = currentSaveType;
    } 

    public static void NextLevel() {

        // Stages 1, 2, 5, 7, and 8 have 5 randomly-generated levels
        if (GetStage() is 1 or 2 or 5 or 7 or 8) {
            
            // If level is right before boss level, load the boss level
            if (GetLevel() == 4) {
                IncrementLevel(1);

                // Load appropriate boss level
                TransitionManager.StartLeaf(dialogueManager.sceneInfo.bossSceneIndexes[GetStage() - 1]);
            } 
            // Else if last level in the stage has been reached, increment stage and set level to 1, then generate new level
            else if (GetLevel() == 5) {
                SetStage(GetStage() + 1);
                SetLevel(1);

                // Load level
                TransitionManager.StartLeaf(GetStage() + dialogueManager.sceneInfo.sceneOffset);
            } 
            // Else, increment level and generate new level
            else {
                IncrementLevel(1);
                Debug.Log("INCREMENTED");

                // Load level
                TransitionManager.StartLeaf(GetStage() + dialogueManager.sceneInfo.sceneOffset);
            }
        } 
        // All other stages have 6 randomly-generated levels
        else {

            // If level is right before boss level, load the boss level
            if (GetLevel() == 5) {
                IncrementLevel(1);
                
                // Load appropriate boss level
                TransitionManager.StartLeaf(dialogueManager.sceneInfo.bossSceneIndexes[GetStage() - 1]);
            } 
            // Else if last level in the stage has been reached, increment stage and set level to 1, then generate new level
            if (GetLevel() == 6) {
                SetStage(GetStage() + 1);
                SetLevel(1);

                // Load level
                TransitionManager.StartLeaf(GetStage() + dialogueManager.sceneInfo.sceneOffset);
            } 
            // Else, increment level and generate new level
            else {
                IncrementLevel(1);
                Debug.Log("INCREMENTED");
                // Load level
                TransitionManager.StartLeaf(GetStage() + dialogueManager.sceneInfo.sceneOffset);
            }
        }
    }

    private void OnDestroy()
    {
        PreviousScene = gameObject.scene.name;
    }

    public static void TogglePause() {
        if (GetState() == GAMESTATE.PLAYING) {
            SetState(GAMESTATE.PAUSED);
        } else if (GetState() == GAMESTATE.PAUSED) {
            SetState(GAMESTATE.PLAYING);
        }
    }

    public static void GameOver() {
        if (GetState() == GAMESTATE.PLAYING) {
            SetState(GAMESTATE.GAMEOVER);
        }
    }
}
