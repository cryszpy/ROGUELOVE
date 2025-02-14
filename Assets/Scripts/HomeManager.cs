using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{

    public PlayerController playerCont;

    // Player max health
    public static int PlayerDeaths;

    public static List<int> SeenItems = new();
    public static int SeenItemsCount;

    public static List<int> SeenWeapons = new();
    public static int SeenWeaponsCount;

    public static bool TutorialDone;

    public static List<int> SeenCharacterIDs = new();

    public int playerDeathsTracker;
    public bool tutorialTracker;
    public int[] seenItemsTracker;
    public int seenItemsCountTracker;
    public int[] seenWeaponsTracker;
    public int seenWeaponsCountTracker;
    public int[] seenCharacterIDsTracker;
    

    private void Update() {
        playerDeathsTracker = PlayerDeaths;
        tutorialTracker = TutorialDone;

        seenItemsTracker = SeenItems.ToArray();
        seenWeaponsTracker = SeenWeapons.ToArray();
        seenItemsCountTracker = SeenItemsCount;
        seenWeaponsCountTracker = SeenWeaponsCount;

        seenCharacterIDsTracker = SeenCharacterIDs.ToArray();
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

        SeenWeaponsCount = 0;
        SeenWeapons.Clear();

        TutorialDone = false;

        SeenCharacterIDs.Clear();
        GameStateManager.dialogueManager.charactersList.ResetAllCharacterNameReveals();

        Debug.LogWarning("RESET HOME");
    }

    public void LoadHome() {

        FindReferences();
        
        // Load save data
        HomeData data = SaveSystem.LoadHome();

        PlayerDeaths = data.playerDeaths;

        // Seen items
        SeenItemsCount = data.seenItemsCount;
        SeenItems = new(data.seenItemsIDs); // Copies over entire array to static list

        // Seen weapons
        SeenWeaponsCount = data.seenWeaponsCount;
        SeenWeapons = new(data.seenWeaponsIDs); // Copies over entire array to static list

        TutorialDone = data.tutorialDone;

        // Character name reveals
        SeenCharacterIDs = new(data.seenCharacterIDs);

        Debug.LogWarning("LOADED HOME");
    }
}
