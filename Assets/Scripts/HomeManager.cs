using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HomeManager : MonoBehaviour
{

    public PlayerController playerCont;

    // Player max health
    public static int PlayerDeaths;
    public int playerDeathsTracker;

    public static List<int> SeenItems = new();
    public static int SeenItemsCount;

    public static bool TutorialDone;
    public bool tutorialTracker;

    private void Update() {
        playerDeathsTracker = PlayerDeaths;
        tutorialTracker = TutorialDone;
    }

    private void FindReferences() {
        if (playerCont == null) {
            playerCont = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            Debug.Log("Player controller was null! Reassigned.");
        }
    }

    public void SaveHome () {
        FindReferences();
        SaveSystem.SaveHome(playerCont.saveIcon);
        Debug.LogWarning("SAVED HOME");
    }

    public void ResetHome() {

        FindReferences();

        PlayerDeaths = 0;

        SeenItemsCount = 0;
        SeenItems.Clear();

        TutorialDone = false;

        Debug.LogWarning("RESET HOME");
    }

    public void LoadHome() {

        FindReferences();
        
        // Load save data
        HomeData data = SaveSystem.LoadHome();

        PlayerDeaths = data.playerDeaths;

        SeenItemsCount = data.seenItemsCount;
        SeenItems = new(data.seenItemsID);

        TutorialDone = data.tutorialDone;

        Debug.LogWarning("LOADED HOME");
    }
}
