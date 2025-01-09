using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class HomeData
{

    public bool tutorialDone;

    public int playerDeaths;

    public int seenItemsCount;
    public int[] seenItemsIDs;

    public int seenWeaponsCount;
    public int[] seenWeaponsIDs;

    public HomeData () {

        // Tutorial status
        tutorialDone = HomeManager.TutorialDone;

        // Player deaths
        playerDeaths = HomeManager.PlayerDeaths;

        // Seen items
        seenItemsCount = HomeManager.SeenItemsCount;
        seenItemsIDs = new int[seenItemsCount];
        for (int i = 0; i < seenItemsCount; i++) {
            seenItemsIDs[i] = HomeManager.SeenItems[i];
        }

        // Seen weapons
        seenWeaponsCount = HomeManager.SeenWeaponsCount;
        seenWeaponsIDs = new int[seenWeaponsCount];
        for (int i = 0; i < seenWeaponsCount; i++) {
            seenWeaponsIDs[i] = HomeManager.SeenWeapons[i];
        }
        
    }
}
