using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PlayerController playerCont;

    [SerializeField] private GameObject saveSlots;

    [SerializeField] private SceneInfo sceneInfo;

    [SerializeField] private GameObject mainMenuScreen;

    private void Awake() {
        Debug.Log(GameStateManager.GetState());

        // If the HOME scene is loaded after a player death, don't show the main menu
        if (GameStateManager.GetState() == GAMESTATE.GAMEOVER) {
            mainMenuScreen.SetActive(false);
            TransitionManager.EndLeaf(true);
            StartHome();
        } 
        // Otherwise, show main menu
        else {
            mainMenuScreen.SetActive(true);
            GameStateManager.SetStage(0);
            GameStateManager.SetLevel(0);
            GameStateManager.SetState(GAMESTATE.MAINMENU);
            Cursor.visible = true;
        }
    }

    // Called AFTER scene is loaded and main menu visibility checks have completed
    public void StartHome() {
        Cursor.visible = false;

        GameStateManager.SetState(GAMESTATE.PLAYING);

        if (playerCont != null) {
            Debug.Log("plyayy");
            playerCont.PlayerStart(true);
        }

        PlayerController.EOnNewPickup += GameStateManager.homeManager.SaveHome;

        GameStateManager.homeManager.SaveHome();
    }

    public void LoadMainMenu() {
        TransitionManager.StartLeaf(0);
    }

    public void PlayButton() {

        string pathHome = Application.persistentDataPath + "/home.soni";

        // Player has an existing save slot
        if (File.Exists(pathHome)) {
            saveSlots.SetActive(true);
        }
        // Player does NOT have an existing save slot (START NEW GAME + TUTORIAL)
        else {

            // Reset saved profile stats if there is no profile
            HomeManager.SeenItems.Clear();
            HomeManager.SeenItemsCount = 0;
            HomeManager.PlayerDeaths = 0;

            GameStateManager.SetSave(false);

            MainMenuTransition(true);
            Debug.Log("TUTORIAL");
        }
    }

    public void SaveSlotButton() {

        string pathMap = Application.persistentDataPath + "/map.chris";
        string pathPlayer = Application.persistentDataPath + "/player.franny";

        // Player has an active run in progress
        if (File.Exists(pathMap) && File.Exists(pathPlayer)) {

            // Load save data to get level number
            MapData data = SaveSystem.LoadMap();

            GameStateManager.SetLevel(data.levelNum);
            GameStateManager.SetStage(data.stageNum);

            GameStateManager.SetSave(true);

            // Load level
            TransitionManager.StartLeaf(GameStateManager.GetStage() + sceneInfo.sceneOffset);
        }
        // Player does NOT have an active run in progress
        else {
            GameStateManager.SetSave(false);

            MainMenuTransition(false);
            Debug.Log("WENT TO SAVED HOME");
        }
    }

    public void MainMenuTransition(bool tutorialStatus) {

        switch (tutorialStatus) {
            case true:
                Debug.Log("Started tutorial transition!");
                // TODO: Implement beginning cutscene

                mainMenuScreen.SetActive(false);
                break;
            case false:
                Debug.Log("Started main menu transition!");
                mainMenuScreen.SetActive(false);
                break;
        }

        StartHome();
    }

    public void QuitButton() {
        Debug.Log("QUIT!");
        Application.Quit();
    }


}
