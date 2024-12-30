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
    public int[] seenItemsID;

    public HomeData () {

        tutorialDone = HomeManager.TutorialDone;

        playerDeaths = HomeManager.PlayerDeaths;

        seenItemsCount = HomeManager.SeenItemsCount;
        seenItemsID = new int[seenItemsCount];
        for (int i = 0; i < seenItemsCount; i++) {
            seenItemsID[i] = HomeManager.SeenItems[i];
        }
        
    }
}
