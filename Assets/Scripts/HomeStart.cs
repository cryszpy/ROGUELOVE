using UnityEngine;

public class HomeStart : MonoBehaviour
{
    // SAVE FILE PATH
    private string pathHome;

    [SerializeField] private PlayerController playerCont;

    [SerializeField] private HomeManager homeManager;

    void Awake() {

        GameStateManager.SetState(GAMESTATE.PLAYING);

        if (homeManager == null) {
            homeManager = GameObject.FindGameObjectWithTag("GameStateManager").GetComponent<HomeManager>();
            Debug.Log("Home manager was null! Reassigned.");
        }

        pathHome = Application.persistentDataPath + "/home.soni";

        if (playerCont != null) {
            Debug.Log("plyayy");
            playerCont.PlayerStart(true);
        }

        PlayerController.EOnNewPickup += homeManager.SaveHome;

        homeManager.SaveHome();

        TransitionManager.EndLeaf(true);
    }
}
