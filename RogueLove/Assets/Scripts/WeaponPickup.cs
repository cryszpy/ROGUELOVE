using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponObject;

    public int weaponID;

    private PlayerController player;
    
    private bool playerFound;

    private bool playerInRadius;

    [SerializeField] private WeaponRarity weaponObjectRarity;

    private void Awake() {
        playerFound = false;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {

            playerInRadius = true;

            if (collider.gameObject.TryGetComponent<PlayerController>(out var controller)) {
                player = controller;
                playerFound = true;
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            playerInRadius = false;
        }
    }

    private void Update() {

        if (!playerFound) {
            return;
        } else if (Input.GetKeyDown(KeyCode.E) && playerFound && player.heldWeapons.Count < 2 && playerInRadius) {

            GameObject weapon = Instantiate(weaponObject, player.weaponPivot.transform.position, Quaternion.identity, player.weaponPivot.transform);
            player.heldWeapons.Add(weapon);
            
            // If picked up weapon goes into the primary weapon slot
            if (player.heldWeapons[0] == weapon) {
                Debug.Log("numba 1");

                PlayerController.PrimaryWeaponRarity = weaponObjectRarity;

                PlayerController.PrimaryWeaponID = player.lootList.drawnWeaponID;

                if (weapon.TryGetComponent<Weapon>(out var primary)) {
                    PlayerController.PrimaryWeaponCurrentAmmo = primary.currentAmmo;
                }
            } 
            // If picked up weapon goes into the secondary weapon slot
            else if (player.heldWeapons[1] == weapon) {
                Debug.Log("numba 2");

                PlayerController.SecondaryWeaponRarity = weaponObjectRarity;

                PlayerController.SecondaryWeaponID = player.lootList.drawnWeaponID;

                if (weapon.TryGetComponent<Weapon>(out var secondary)) {
                    PlayerController.SecondaryWeaponCurrentAmmo = secondary.currentAmmo;
                }
            }

            player.StartWeaponSwitch(1, PlayerController.CurrentWeaponIndex);
            RemoveObject();
        }
    }

    private void RemoveObject() {
        Destroy(gameObject);
    }
}
