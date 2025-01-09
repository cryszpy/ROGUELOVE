using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    protected WalkerGenerator map;

    [SerializeField] protected float itemChance;

    [SerializeField] protected Collider2D coll;

    [SerializeField] protected Animator animator;

    [SerializeField] protected LootList lootList;

    [SerializeField] protected ItemList itemList;

    protected bool playerInRadius;

    public virtual void Awake() {
        if (coll.enabled == false) {
            coll.enabled = true;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collider) {

        // If player is within radius, light up
        if (collider.CompareTag("Player")) {
            playerInRadius = true;
            animator.SetBool("Glow", true);
        } 
    }

    public virtual void OnTriggerExit2D(Collider2D collider) {

        // If player leaves radius, stop glow
        if (collider.CompareTag("Player")) {
            playerInRadius = false;
            animator.SetBool("Glow", false);
        } 
    }

    public virtual void Update() {

        // Open chest
        if (playerInRadius && Input.GetKeyDown(KeyCode.E)) {

            // Makes sure this isn't the tutorial chest
            if (GameStateManager.GetStage() != 1) {

                if (map) {
                    map.spawnedChests.Remove(gameObject);

                    Debug.Log("Opened chest and successfully removed from list!");
                } else {
                    Debug.LogError("WalkerGenerator component of chest not assigned!");
                }
            }

            float rand = UnityEngine.Random.value;

            coll.enabled = false;

            if (rand <= itemChance) {
                OpenItemChest();
            } else {
                OpenWeaponChest();
            }
        }
    }

    public virtual void OpenItemChest() {

        // Generate a random value between 0.0 and 1.0 (to generate rarity of item)
        float rand = Random.value;

        GetItemProbability(rand, itemList.areaRarities[GameStateManager.GetStage() - 1].area);
    }

    public virtual void OpenWeaponChest() {

        // For dev testing only, should be null upon build
        if (lootList.testingWeapon.pickupScript != null) {

            WeaponPair weaponPair = lootList.testingWeapon;
            weaponPair.pickupScript.dropped = false;
            Instantiate(weaponPair.pickupObject, transform.position, Quaternion.identity);

            lootList.drawnWeaponID = weaponPair.pickupScript.weaponID;

            WeaponPair listItem = lootList.GetSpecificWeapon(weaponPair, weaponPair.pickupScript.weaponObjectRarity);

            lootList.RemoveWeapon(listItem, listItem.pickupScript.weaponObjectRarity);
        } else {

            // Generate a random value between 0.0 and 1.0 (to generate rarity of weapon)
            float rand = Random.value;

            GetWeaponProbability(rand, lootList.areaRarities[GameStateManager.GetStage() - 1].area);
        }
    }

    public virtual void GetWeaponProbability(float value, float[] lootTable) {

        if (value <= lootTable[0]) {
            if (lootList.commonWeapons.Count != 0) {
                WeaponPair weaponPair = lootList.GetRandomWeapon(WeaponRarity.COMMON);
                weaponPair.pickupScript.dropped = false;
                Instantiate(weaponPair.pickupObject, transform.position, Quaternion.identity);

                lootList.drawnWeaponID = weaponPair.pickupScript.weaponID;

                lootList.RemoveWeapon(weaponPair, WeaponRarity.COMMON);
            } else {
                Debug.Log("Would've spawned a COMMON weapon but there are none!");
            }
        }
        else if (value <= lootTable[1]) {
            if (lootList.uncommonWeapons.Count != 0) {
                WeaponPair weaponPair = lootList.GetRandomWeapon(WeaponRarity.UNCOMMON);
                weaponPair.pickupScript.dropped = false;
                Instantiate(weaponPair.pickupObject, transform.position, Quaternion.identity);

                lootList.drawnWeaponID = weaponPair.pickupScript.weaponID;

                lootList.RemoveWeapon(weaponPair, WeaponRarity.UNCOMMON);
            } else {
                Debug.Log("Would've spawned an UNCOMMON weapon but there are none!");
            }
        }
        else if (value <= lootTable[2]) {
            if (lootList.rareWeapons.Count != 0) {
                WeaponPair weaponPair = lootList.GetRandomWeapon(WeaponRarity.RARE);
                weaponPair.pickupScript.dropped = false;
                Instantiate(weaponPair.pickupObject, transform.position, Quaternion.identity);

                lootList.drawnWeaponID = weaponPair.pickupScript.weaponID;

                lootList.RemoveWeapon(weaponPair, WeaponRarity.RARE);
            } else {
                Debug.Log("Would've spawned a RARE weapon but there are none!");
            }
        }
        else if (value <= lootTable[3]) {
            if (lootList.epicWeapons.Count != 0) {
                WeaponPair weaponPair = lootList.GetRandomWeapon(WeaponRarity.EPIC);
                weaponPair.pickupScript.dropped = false;
                Instantiate(weaponPair.pickupObject, transform.position, Quaternion.identity);

                lootList.drawnWeaponID = weaponPair.pickupScript.weaponID;

                lootList.RemoveWeapon(weaponPair, WeaponRarity.EPIC);
            } else {
                Debug.Log("Would've spawned an EPIC weapon but there are none!");
            }
        }
        else {
            if (lootList.legendaryWeapons.Count != 0) {
                WeaponPair weaponPair = lootList.GetRandomWeapon(WeaponRarity.LEGENDARY);
                weaponPair.pickupScript.dropped = false;
                Instantiate(weaponPair.pickupObject, transform.position, Quaternion.identity);

                lootList.drawnWeaponID = weaponPair.pickupScript.weaponID;

                lootList.RemoveWeapon(weaponPair, WeaponRarity.LEGENDARY);
            } else {
                Debug.Log("Would've spawned a LEGENDARY weapon but there are none!");
            }
        }
    }

    public virtual void GetItemProbability(float value, float[] lootTable) {

        if (value <= lootTable[0]) {
            if (itemList.commonItems.Count != 0) {
                ItemPickup item = itemList.GetRandomItem(ItemRarity.COMMON);
                item.dropped = false;
                Instantiate(item, transform.position, Quaternion.identity);

                itemList.drawnItemID = item.itemID;

                itemList.RemoveItem(item, ItemRarity.COMMON);
            } else {
                Debug.Log("Would've spawned a COMMON item but there are none!");
            }
        }
        else if (value <= lootTable[1]) {
            if (itemList.uncommonItems.Count != 0) {
                ItemPickup item = itemList.GetRandomItem(ItemRarity.UNCOMMON);
                item.dropped = false;
                Instantiate(item, transform.position, Quaternion.identity);

                itemList.drawnItemID = item.itemID;

                itemList.RemoveItem(item, ItemRarity.UNCOMMON);
            } else {
                Debug.Log("Would've spawned a UNCOMMON item but there are none!");
            }
        }
        else if (value <= lootTable[2]) {
            if (itemList.rareItems.Count != 0) {
                ItemPickup item = itemList.GetRandomItem(ItemRarity.RARE);
                item.dropped = false;
                Instantiate(item, transform.position, Quaternion.identity);

                itemList.drawnItemID = item.itemID;

                itemList.RemoveItem(item, ItemRarity.RARE);
            } else {
                Debug.Log("Would've spawned a RARE item but there are none!");
            }
        }
        else if (value <= lootTable[3]) {
            if (itemList.epicItems.Count != 0) {
                ItemPickup item = itemList.GetRandomItem(ItemRarity.EPIC);
                item.dropped = false;
                Instantiate(item, transform.position, Quaternion.identity);

                itemList.drawnItemID = item.itemID;

                itemList.RemoveItem(item, ItemRarity.EPIC);
            } else {
                Debug.Log("Would've spawned a EPIC item but there are none!");
            }
        }
        else {
            if (itemList.legendaryItems.Count != 0) {
                ItemPickup item = itemList.GetRandomItem(ItemRarity.LEGENDARY);
                item.dropped = false;
                Instantiate(item, transform.position, Quaternion.identity);

                itemList.drawnItemID = item.itemID;

                itemList.RemoveItem(item, ItemRarity.LEGENDARY);
            } else {
                Debug.Log("Would've spawned a LEGENDARY item but there are none!");
            }
        }
    }

    public virtual UnityEngine.Object Create(UnityEngine.Object original, Vector3 position, Quaternion rotation, WalkerGenerator gen) {
        GameObject chest = Instantiate(original, position, rotation) as GameObject;
        
        if (chest.TryGetComponent<Chest>(out var script)) {
            script.map = gen;
            Debug.Log("Chest spawned!");
            return chest;
        } else {
            Debug.LogError("Could not find Chest script or extension of such on this Object.");
            return null;
        }
    }
}
