
[System.Serializable]
public class HomeData
{

    public bool tutorialDone;

    public int playerDeaths;

    public int seenItemsCount;
    public int[] seenItemsIDs;

    public int seenWeaponsCount;
    public int[] seenWeaponsIDs;

    public int[] seenCharacterIDs;

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

        // Character name reveals
        seenCharacterIDs = new int[HomeManager.SeenCharacterIDs.Count];
        for (int i = 0; i < HomeManager.SeenCharacterIDs.Count; i++) {
            seenCharacterIDs[i] = HomeManager.SeenCharacterIDs[i];
        }
    }
}
