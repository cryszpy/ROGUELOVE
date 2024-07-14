using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private GameObject weaponObject;

    private PlayerController player;
    
    private bool playerFound;

    private bool playerInRadius;

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
            player.StartWeaponSwitch(1, player.currentWeaponIndex);
            RemoveObject();
        }
    }

    private void RemoveObject() {
        Destroy(gameObject);
    }
}
