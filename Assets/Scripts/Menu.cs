using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    GameObject saveSlots;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField] private SceneInfo sceneInfo;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null) {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu() {
        // Unpause
        if (GameStateManager.GetState() == GAMESTATE.PAUSED) {
            GameStateManager.TogglePause();
            GameStateManager.SetState(GAMESTATE.PLAYING);
            pauseMenu.SetActive(false);
        } 
        // Pause
        else if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            GameStateManager.TogglePause();
            GameStateManager.SetState(GAMESTATE.PAUSED);
            pauseMenu.SetActive(true);
        }
    }

    public void LoadMainMenu() {
        TransitionManager.StartLeaf(0);
    }

    public void PlayButton() {
        string pathMap = Application.persistentDataPath + "/map.chris";
        string pathPlayer = Application.persistentDataPath + "/player.franny";
        string pathHome = Application.persistentDataPath + "/home.soni";

        // Reset saved profile stats if there is no profile
        if (!File.Exists(pathHome)) {
            HomeManager.SeenItems.Clear();
            HomeManager.SeenItemsCount = 0;
            HomeManager.PlayerDeaths = 0;
        }

        // Set up SAVED GAME
        if (File.Exists(pathMap) && File.Exists(pathPlayer)) {
            saveSlots.SetActive(true);
        } 
        // NO SAVED RUN, GO HOME
        else if (File.Exists(pathHome)) {
            GameStateManager.SetSave(false);

            TransitionManager.StartLeaf(1);
            Debug.Log("WENT TO SAVED HOME");
        }
        // START NEW GAME + TUTORIAL
        else {
            GameStateManager.SetSave(false);

            TransitionManager.StartLeaf(1);
            Debug.Log("TUTORIAL");
        }
    }

    public void SaveSlotButton() {
        // Load save data to get level number
        MapData data = SaveSystem.LoadMap();

        GameStateManager.SetLevel(data.levelNum);
        GameStateManager.SetStage(data.stageNum);

        GameStateManager.SetSave(true);

        // Load level
        TransitionManager.StartLeaf(GameStateManager.GetStage() + sceneInfo.sceneOffset);
    }

    public void QuitButton() {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
