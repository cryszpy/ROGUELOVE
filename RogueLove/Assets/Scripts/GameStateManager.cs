using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// State of the game
public enum GAMESTATE {
    MENU, PLAYING, PAUSED, GAMEOVER
}

public class GameStateManager : MonoBehaviour
{
    public static string PreviousScene { get; private set; }

    // Reference to the active instance of this object
    private static GameStateManager instance;

    // Dialogue Manager attached to this GameObject
    public static DialogueManager dialogueManager;

    // Methods to check the state of the game
    private static GAMESTATE state;
    public static GAMESTATE GetState() {
        return state;
    }
    public static void SetState(GAMESTATE newState) {
        state = newState;
        Time.timeScale = state switch
        {
            GAMESTATE.MENU => 1,
            GAMESTATE.GAMEOVER => 1,
            GAMESTATE.PAUSED => 0,
            GAMESTATE.PLAYING => 1,
            _ => (float)1,
        };
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

    public static SceneList sceneList;

    public static bool levelClear = false;
    public static void SetLevelClear(bool condition) {
        levelClear = condition;
    }
    public static bool GetLevelClear() {
        return levelClear;
    }

    void Start() {

        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            dialogueManager = this.GetComponent<DialogueManager>();
            DontDestroyOnLoad(gameObject);
        }

        if (GetState() == 0) {
            SetState(GAMESTATE.MENU);
        } else {
            SetState(GAMESTATE.PLAYING);
        }
        
        sceneList = this.gameObject.GetComponent<SceneList>();

        //Debug.Log("previous scene: " + GameStateManager.PreviousScene);  // use this in any level to get the last level.
    }

    /* void Update() {
        if (state != GAMESTATE.PAUSED && state != GAMESTATE.GAMEOVER) {
            Time.timeScale = 1;
        }
    } */

    public static void NextLevel() {

        // Stages 1, 2, 5, 7, and 8 have 5 randomly-generated levels
        if (GetStage() is 1 or 2 or 5 or 7 or 8) {
            // If last level in the stage has been reached, increment stage and set level to 1, then generate new level
            if (GetLevel() == 5) {
                SetStage(GetStage() + 1);
                SetLevel(1);
                // Load level
                TransitionManager.StartLeaf(GetStage());
            } 
            // Else, increment level and generate new level
            else {
                IncrementLevel(1);
                Debug.Log("INCREMENTED");
                // Load level
                TransitionManager.StartLeaf(GetStage());
            }
        } 
        // All other stages have 6 randomly-generated levels
        else {
            // If last level in the stage has been reached, increment stage and set level to 1, then generate new level
            if (GetLevel() == 6) {
                SetStage(GetStage() + 1);
                SetLevel(1);
                // Load level
                TransitionManager.StartLeaf(GetStage());
            } 
            // Else, increment level and generate new level
            else {
                IncrementLevel(1);
                Debug.Log("INCREMENTED");
                // Load level
                TransitionManager.StartLeaf(GetStage());
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
