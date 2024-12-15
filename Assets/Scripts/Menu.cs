using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    [SerializeField] private GameObject pauseMenu;

    private void Start() {
        if (!player) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            Debug.LogWarning("PlayerController component was null! Reassigned.");
        }
    }

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

    public void AbandonRun() {
        GameStateManager.SetState(GAMESTATE.GAMEOVER);
        
        player.ResetRun();
    }

    public void LoadMainMenu() {
        GameStateManager.SetState(GAMESTATE.PAUSED);
        
        player.ResetRun();
    }
}
