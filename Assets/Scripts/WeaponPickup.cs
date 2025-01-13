using UnityEngine;

[System.Serializable]
public class WeaponPair {
    public GameObject pickupObject;
    public WeaponPickup pickupScript;
    public float dropChance;
}

public class WeaponPickup : BasePickup, IPickupable
{

    public Weapon parent;

    public WeaponRarity weaponObjectRarity;

    public int weaponID;

    public virtual void OnEnable() {
        PlayerController.EOnWeaponPickup += PickupSound;
    }

    public virtual void OnDisable() {
        PlayerController.EOnWeaponPickup -= PickupSound;
    }

    public override void Update() {

        if (!playerFound) {
            return;
        } 
        // Pickup weapon
        else if (Input.GetKeyDown(KeyCode.E) && playerFound && player.heldWeapons.Count < 2 && playerInRadius) {

            Pickup(false);
            player.StartWeaponSwitch(player.heldWeapons.Count - 1 - PlayerController.CurrentWeaponIndex, PlayerController.CurrentWeaponIndex);
            RemoveObject();
        } 
        // Replace weapons
        else if (Input.GetKeyDown(KeyCode.E) && playerFound && player.heldWeapons.Count == 2 && player.heldWeapons[PlayerController.CurrentWeaponIndex] != null 
        && playerInRadius) {
            if (player.heldWeapons[PlayerController.CurrentWeaponIndex].TryGetComponent<Weapon>(out var script)) {
                player.DropWeapon(script, true);
                Pickup(true);
                player.StartWeaponSwitch(PlayerController.CurrentWeaponIndex, player.heldWeapons.Count - 1 - PlayerController.CurrentWeaponIndex);
                RemoveObject();
            }
        }
    }

    public void Pickup(bool replace) {

        if (dropped) {
            objectToSpawn.transform.position = player.weaponPivot.transform.position;
            objectToSpawn.transform.parent = player.weaponPivot.transform;

            if (replace) {
                player.heldWeapons[PlayerController.CurrentWeaponIndex] = objectToSpawn;
            } else {
                player.heldWeapons.Add(objectToSpawn);
            }

            UpdateWeapon(objectToSpawn);
        } else {

            // If this is the first time picking up the weapon ever, add it to the new weapons list and trigger UI animation
            if (!HomeManager.SeenWeapons.Contains(weaponID)) {
                HomeManager.SeenWeapons.Add(weaponID);
                HomeManager.SeenWeaponsCount++;
                GameStateManager.pickupManager.StartWeaponAnimation(parent);
                PlayerController.EOnNewPickup?.Invoke(); // Trigger any OnNewPickup events
            }

            GameObject weapon = Instantiate(objectToSpawn, player.weaponPivot.transform.position, Quaternion.identity, player.weaponPivot.transform);

            if (replace) {
                player.heldWeapons[PlayerController.CurrentWeaponIndex] = weapon;
            } else {
                player.heldWeapons.Add(weapon);
            }

            UpdateWeapon(weapon);
        }

        PlayerController.EOnWeaponPickup?.Invoke();
    }

    public override void PickupSound()
    {
        AudioManager.instance.PlaySoundByName("weapon_pickup", transform);
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
                player.ammoBar.weaponSprite.sprite = primary.spriteRenderer.sprite;
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
                player.ammoBar.weaponSprite.sprite = secondary.spriteRenderer.sprite;
            }
        }
    }
}
