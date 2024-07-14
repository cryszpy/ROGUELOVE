using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChestType {
    WEAPON, ITEM
}

public class Chest : MonoBehaviour
{
    [SerializeField] private ChestType type;

    [SerializeField] private Collider2D coll;

    [SerializeField] private Animator animator;

    [SerializeField] private LootList lootList;

    private bool playerInRadius;

    void Awake() {
        if (coll.enabled == false) {
            coll.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        // If player is within radius, light up
        if (collider.CompareTag("Player")) {
            playerInRadius = true;
            animator.SetBool("Glow", true);
        } 
    }

    private void OnTriggerExit2D(Collider2D collider) {

        // If player leaves radius, stop glow
        if (collider.CompareTag("Player")) {
            playerInRadius = false;
            animator.SetBool("Glow", false);
        } 
    }

    private void Update() {
        if (playerInRadius && Input.GetKeyDown(KeyCode.E)) {

            switch (type) {
                case ChestType.WEAPON:
                    coll.enabled = false;
                    OpenWeaponChest();
                    break;
                case ChestType.ITEM:
                    coll.enabled = false;
                    OpenItemChest();
                    break;
            }
        }
    }

    private void OpenItemChest() {
        int rand = Random.Range(0, lootList.items.Count - 1);
        
    }

    public void OpenWeaponChest() {

        // Generate a random value between 0.0 and 1.0 (to generate rarity of weapon)
        float rand = Random.value;

        switch (GameStateManager.GetStage()) {
            case 1:
                // Area 1 - Forest Clearing
                GetWeaponProbability(rand, lootList.areaOne);
                break;
            case 2:
                // Area 2 - Sandy Shores
                GetWeaponProbability(rand, lootList.areaTwo);
                break;
            case 3:
                // Area 3 - Aisles Warehouse
                GetWeaponProbability(rand, lootList.areaThree);
                break;
            case 4:
                // Area 4 - Bellicose Street
                GetWeaponProbability(rand, lootList.areaFour);
                break;
            case 5:
                // Area 5 - Snow-Capped Tundra
                GetWeaponProbability(rand, lootList.areaFive);
                break;
            case 6:
                // Area 6 - Magical Greenwood
                GetWeaponProbability(rand, lootList.areaSix);
                break;
            case 7:
                // Area 7 - Waterfall City
                GetWeaponProbability(rand, lootList.areaSeven);
                break;
            case 8:
                // Area 8 - Fallow's Father's Mansion
                GetWeaponProbability(rand, lootList.areaEight);
                break;
        }
    }

    private void GetWeaponProbability(float value, float[] lootTable) {

        if (value <= lootTable[0]) {
            if (lootList.commonWeapons.Count != 0) {
                var weapon = lootList.GetRandomWeapon(WeaponRarity.COMMON);
                Instantiate(weapon, transform.position, Quaternion.identity);
                lootList.RemoveWeapon(weapon, WeaponRarity.COMMON);
            } else {
                Debug.Log("Would've spawned a COMMON weapon but there are none!");
            }
        }
        else if (value <= lootTable[1]) {
            if (lootList.uncommonWeapons.Count != 0) {
                var weapon = lootList.GetRandomWeapon(WeaponRarity.UNCOMMON);
                Instantiate(weapon, transform.position, Quaternion.identity);
                lootList.RemoveWeapon(weapon, WeaponRarity.UNCOMMON);
            } else {
                Debug.Log("Would've spawned an UNCOMMON weapon but there are none!");
            }
        }
        else if (value <= lootTable[2]) {
            if (lootList.rareWeapons.Count != 0) {
                var weapon = lootList.GetRandomWeapon(WeaponRarity.RARE);
                Instantiate(weapon, transform.position, Quaternion.identity);
                lootList.RemoveWeapon(weapon, WeaponRarity.RARE);
            } else {
                Debug.Log("Would've spawned a RARE weapon but there are none!");
            }
        }
        else if (value <= lootTable[3]) {
            if (lootList.epicWeapons.Count != 0) {
                var weapon = lootList.GetRandomWeapon(WeaponRarity.EPIC);
                Instantiate(weapon, transform.position, Quaternion.identity);
                lootList.RemoveWeapon(weapon, WeaponRarity.EPIC);
            } else {
                Debug.Log("Would've spawned an EPIC weapon but there are none!");
            }
        }
        else {
            if (lootList.legendaryWeapons.Count != 0) {
                var weapon = lootList.GetRandomWeapon(WeaponRarity.LEGENDARY);
                Instantiate(weapon, transform.position, Quaternion.identity);
                lootList.RemoveWeapon(weapon, WeaponRarity.LEGENDARY);
            } else {
                Debug.Log("Would've spawned a LEGENDARY weapon but there are none!");
            }
        }
    }
}
