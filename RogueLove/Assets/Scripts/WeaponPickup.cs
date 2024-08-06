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

    public bool dropped = false;

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
        } 
        // Pickup weapon
        else if (Input.GetKeyDown(KeyCode.E) && playerFound && player.heldWeapons.Count < 2 && playerInRadius) {

            PickupWeapon(false);

            player.StartWeaponSwitch(player.heldWeapons.Count - 1 - PlayerController.CurrentWeaponIndex, PlayerController.CurrentWeaponIndex);
            RemoveObject();
        } 
        // Replace weapons
        else if (Input.GetKeyDown(KeyCode.E) && playerFound && player.heldWeapons.Count == 2 && player.heldWeapons[PlayerController.CurrentWeaponIndex] != null 
        && playerInRadius) {
            if (player.heldWeapons[PlayerController.CurrentWeaponIndex].TryGetComponent<Weapon>(out var script)) {
                player.DropWeapon(script, true);
                PickupWeapon(true);
                player.StartWeaponSwitch(PlayerController.CurrentWeaponIndex, player.heldWeapons.Count - 1 - PlayerController.CurrentWeaponIndex);
                RemoveObject();
            }
        }
    }

    public void PickupWeapon(bool replace) {

        if (dropped) {
            weaponObject.transform.position = player.weaponPivot.transform.position;
            weaponObject.transform.parent = player.weaponPivot.transform;

            if (replace) {
                player.heldWeapons[PlayerController.CurrentWeaponIndex] = weaponObject;
            } else {
                player.heldWeapons.Add(weaponObject);
            }

            UpdateWeapon(weaponObject);
        } else {
            GameObject weapon = Instantiate(weaponObject, player.weaponPivot.transform.position, Quaternion.identity, player.weaponPivot.transform);

            if (replace) {
                player.heldWeapons[PlayerController.CurrentWeaponIndex] = weapon;
            } else {
                player.heldWeapons.Add(weapon);
            }

            UpdateWeapon(weapon);
        }
    }

    private void UpdateWeapon(GameObject weapon) {

        // If picked up weapon goes into the primary weapon slot
        if (player.heldWeapons[0] == weapon) {

            PlayerController.PrimaryWeaponRarity = weaponObjectRarity;

            if (weapon.TryGetComponent<Weapon>(out var primary)) {
                PlayerController.PrimaryWeaponID = primary.id;
                PlayerController.PrimaryWeaponCurrentAmmo = primary.currentAmmo;
                player.ammoBar.SetMaxAmmo(primary.ammoMax * PlayerController.AmmoMaxMultiplier);
                player.ammoBar.SetAmmo(PlayerController.PrimaryWeaponCurrentAmmo, primary);
                player.ammoBar.weaponSprite.sprite = primary.sprite.sprite;
            }
        } 
        // If picked up weapon goes into the secondary weapon slot
        else if (player.heldWeapons[1] == weapon) {

            PlayerController.SecondaryWeaponRarity = weaponObjectRarity;

            if (weapon.TryGetComponent<Weapon>(out var secondary)) {
                PlayerController.SecondaryWeaponID = secondary.id;
                PlayerController.SecondaryWeaponCurrentAmmo = secondary.currentAmmo;
                player.ammoBar.SetMaxAmmo(secondary.ammoMax * PlayerController.AmmoMaxMultiplier);
                player.ammoBar.SetAmmo(PlayerController.SecondaryWeaponCurrentAmmo, secondary);
                player.ammoBar.weaponSprite.sprite = secondary.sprite.sprite;
            }
        }
    }

    private void RemoveObject() {
        Destroy(gameObject);
    }
}
