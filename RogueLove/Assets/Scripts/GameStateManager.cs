using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static string PreviousScene { get; private set; }

    // Reference to the active instance of this object
    private static GameStateManager instance;

    // State of the game
    public enum GAMESTATE {
        MENU, PLAYING, PAUSED, GAMEOVER
    }

    // Methods to check the state of the game
    private static GAMESTATE state;
    public static GAMESTATE GetState() {
        return state;
    }
    public static void SetState(GAMESTATE newState) {
        state = newState;
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
            DontDestroyOnLoad(gameObject);
        }

        state = GAMESTATE.PLAYING;

        Debug.Log("current level: " + currentLevel);
        Debug.Log("current stage: " + currentStage);
        
        sceneList = this.gameObject.GetComponent<SceneList>();

        //Debug.Log("previous scene: " + GameStateManager.PreviousScene);  // use this in any level to get the last level.
    }

    public static void NextLevel() {

        // Stages 1, 2, 5, 7, and 8 have 3 randomly-generated levels
        if (GetStage() is 1 or 2 or 5 or 7 or 8) {
            // If last level in the stage has been reached, increment stage and set level to 1, then generate new level
            if (GetLevel() == 3) {
                SetStage(GetStage() + 1);
                SetLevel(1);
                // Load level
                TransitionManager.StartLeaf(GetStage());
            } 
            // Else, increment level and generate new level
            else {
                SetLevel(GetLevel() + 1);
                // Load level
                TransitionManager.StartLeaf(GetStage());
            }
        } 
        // All other stages have 4 randomly-generated levels
        else {
            // If last level in the stage has been reached, increment stage and set level to 1, then generate new level
            if (GetLevel() == 4) {
                SetStage(GetStage() + 1);
                SetLevel(1);
                // Load level
                TransitionManager.StartLeaf(GetStage());
            } 
            // Else, increment level and generate new level
            else {
                SetLevel(GetLevel() + 1);
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
        if (state == GAMESTATE.PLAYING) {
            state = GAMESTATE.PAUSED;
            Time.timeScale = 0;
        } else if (state == GAMESTATE.PAUSED) {
            state = GAMESTATE.PLAYING;
            Time.timeScale = 1;
        }
    }

    public static void GameOver() {
        if (state == GAMESTATE.PLAYING) {
            state = GAMESTATE.GAMEOVER;
        }
    }
}
