using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{

    public PlayerController playerCont;

    // Player max health
    public static int PlayerDeaths;
    public int playerDeathsTracker;

    public static List<int> SeenItems = new();
    public static int SeenItemsCount;

    private void Update() {
        playerDeathsTracker = PlayerDeaths;
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
        Debug.Log("SAVE HOME CALLED");
    }

    public void LoadHome() {

        FindReferences();
        
        // Load save data
        HomeData data = SaveSystem.LoadHome();

        PlayerDeaths = data.playerDeaths;

        SeenItemsCount = data.seenItemsCount;
        SeenItems = new(data.seenItemsID);

        Debug.Log("LOADED HOME");
    }

}
