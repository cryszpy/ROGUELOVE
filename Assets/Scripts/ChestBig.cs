using UnityEngine;

public class ChestBig : Chest
{
    
    public override void Update() {

        // Open chest
        if (playerInRadius && Input.GetKeyDown(KeyCode.E)) {

            if (map) {
                map.spawnedChests.Remove(gameObject);

                Debug.Log("Opened chest and successfully removed from list!");
            } else {
                Debug.LogError("WalkerGenerator component of chest not assigned!");
            }

            float rand = UnityEngine.Random.value;

            coll.enabled = false;

            if (rand <= itemChance) {
                OpenItemChest();
                OpenWeaponChest();
                OpenItemChest();
            } else {
                OpenWeaponChest();
                OpenItemChest();
                OpenWeaponChest();
            }
        }
    }
}
