using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static string PreviousScene { get; private set; }

    private static GameStateManager instance;

    public enum GAMESTATE {
        MENU, PLAYING, PAUSED, GAMEOVER
    }

    private static GAMESTATE state;

    public static GAMESTATE getState() {
        return state;
    }

    public static void setState(GAMESTATE newState) {
        state = newState;
    }

    public static SceneList sceneList;

    static int currentLvl;

    void Start() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        state = GAMESTATE.PLAYING;

        currentLvl = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("current level: " + currentLvl);

        
        sceneList = this.gameObject.GetComponent<SceneList>();
        /*
        string delimiter = ", ";
        string xx = sceneList.scene.Select(i => i.ToString()).Aggregate((i, j) => i + delimiter + j);
        Debug.Log("sceneList: " + xx);
        */

        Debug.Log("previous scene: " + GameStateManager.PreviousScene);  // use this in any level to get the last level.
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
}
