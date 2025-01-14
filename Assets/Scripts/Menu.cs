using UnityEngine;

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
        if ((GameStateManager.GetState() == GAMESTATE.PAUSED || GameStateManager.GetState() == GAMESTATE.PLAYING) && !GameStateManager.transitionPauseLock) {
            if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null) {
                TogglePauseMenu();
            }
        }
    }

    public void TogglePauseMenu() {
        // Unpause
        if (GameStateManager.GetState() == GAMESTATE.PAUSED) {
            GameStateManager.SetState(GAMESTATE.PLAYING);
            pauseMenu.SetActive(false);
        } 
        // Pause
        else if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            GameStateManager.SetState(GAMESTATE.PAUSED);
            pauseMenu.SetActive(true);
        }
    }

    public void AbandonRun() {
        GameStateManager.SetState(GAMESTATE.GAMEOVER);
        
        player.ResetRun();
        player.IncrementDeath();
    }

    public void LoadMainMenu() {
        GameStateManager.SetState(GAMESTATE.MAINMENU);
        
        player.ResetRun();
        player.IncrementDeath();
    }
}
