using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public delegate void PlayerVoidHandler();
    public static PlayerVoidHandler EOnDamaged;
    public static PlayerVoidHandler EOnDodged;
    public static PlayerVoidHandler EOnMoving;
    public static PlayerVoidHandler EOnEnergyFull;
    public static PlayerVoidHandler EOnDeath;
    public static PlayerVoidHandler EOnItemPickup;
    public static PlayerVoidHandler EOnWeaponPickup;

    [Header("SCRIPT REFERENCES")]

    public LootList lootList;
    public ItemList itemList;

    [SerializeField] private WeaponPair defaultWeapon;

    [SerializeField] private ContactFilter2D movementFilter;

    public Weapon weapon;

    public GameObject weaponPivot;

    public CapsuleCollider2D contactColl;

    public Rigidbody2D rb;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Tooltip("A reference to the player's health meter.")]
    public HealthBar healthBar;

    [Tooltip("A reference to the player's battery meter.")]
    public EnergyBar energyBar;

    [Tooltip("A reference to the player's coins.")]
    public CoinsUI coinsUI;

    [Tooltip("A reference to the player's ammo meter.")]
    public WeaponInfo ammoBar;

    readonly List<RaycastHit2D> castCollisions = new();
    
    private VolumeProfile volumeProfile;

    [SerializeField] private CircleCollider2D dashColl;

    private Camera mainCam;
    public CinemachineImpulseSource camShake;

    public GameObject saveIcon;

    [SerializeField] CameraLookAt cameraLookAt;

    [Header("STATS")]

    // WEAPONS
    [HideInInspector] public List<GameObject> heldWeapons = new();

    public static int CurrentWeaponIndex;

    public static WeaponRarity PrimaryWeaponRarity;

    public static float PrimaryWeaponCurrentAmmo;

    public static WeaponRarity SecondaryWeaponRarity;

    public static float SecondaryWeaponCurrentAmmo;

    public static int PrimaryWeaponID;

    public static int SecondaryWeaponID;

    [Tooltip("A multiplier that increases or decreases the maximum ammo capacity of the player's weapons.")]
    public static float AmmoMaxMultiplier;

    // ITEMS
    [Tooltip("List of all items that the player has picked up.")]
    public List<ItemPickup> heldItems = new();
    public static List<int> HeldItemsID = new();
    public static List<ItemRarity> HeldItemsRarity = new();

    [Tooltip("Total number of items that the player is holding.")]
    public static int HeldItemsCount;

    // HEALTH
    private static int maxHealth;
    public static int MaxHealth { get => maxHealth; set => maxHealth = value; }

    public static void AddMaxHealth(int num) {
        MaxHealth += num;
    }

    // MOVEMENT SPEED
    private static float moveSpeed = 3.2f;
    public static float MoveSpeed { get => moveSpeed; set => moveSpeed = value * MoveSpeedMultiplier; }
    public static void ChangeMoveSpeed(float speed) {
        MoveSpeed += speed;
    }

    private static float moveSpeedMultiplier = 1;
    public static float MoveSpeedMultiplier { get => moveSpeedMultiplier; set => moveSpeedMultiplier = value; }

    public static int currentHealth;

    public int Health {
        set {
            currentHealth = value;
            if(currentHealth <= 0) {
                currentHealth = 0;
                EOnDeath?.Invoke(); // Triggers the OnDeath event
            }
        }

        get {
            return currentHealth;
        }
    }

    // DODGE CHANCE
    [Tooltip("Chance to dodge incoming attacks.")]
    private static float dodgeChance;
    public static float DodgeChance { get => dodgeChance; set => dodgeChance = value;}

    // TAKEN DAMAGE MULTIPLIER
    [Tooltip("Enemy damage reduction multiplier.")]
    private static float takenDamageMult;
    public static float TakenDamageMult { get => takenDamageMult; set => takenDamageMult = value;}

    // ENERGY
    private static float maxEnergy;
    public static float MaxEnergy { get => maxEnergy; set => maxEnergy = value; }

    private static bool chargedBattery = false;

    private static float experience;
    public static float Experience { 
        get => experience; 

        set {
            experience = value;

            if (experience >= maxEnergy) {

                if (!chargedBattery) {
                    chargedBattery = true;

                    // Play battery full animation here!!
                    Debug.Log("Battery full!");
                    EOnEnergyFull?.Invoke();

                    // Add a call dialogue piece if not tutorial
                    if (!GameStateManager.tutorialEnabled) {
                        GameStateManager.dialogueManager.AddCallDialogue(GameStateManager.dialogueManager.callDialogueList, GameStateManager.dialogueManager.player);
                    }

                    // Experience carries over to the next level (still need to make exponentially higher max energy reqs)
                    experience -= maxEnergy;
                }
                
            } else {
                chargedBattery = false;
            }
        }
    }

    public static void AddExperience(float exp) {
        Experience += exp;
    }

    // COINS
    private static int coins;
    public static int Coins { get => coins; set => coins = value; }

    public static void AddCoins(int value) {
        Coins += value;
    }

    // VIEW RANGE
    public static float viewRangeBase;
    public static float ViewRangeBase { get => viewRangeBase; set => viewRangeBase = value; }
    private static float viewRangeMultiplier;
    public static float ViewRangeMultiplier { get => viewRangeMultiplier; set => viewRangeMultiplier = value;}

    // DAMAGE MULTIPLIER
    public static float DamageModifier;

    // FIRE RATE
    public static float FireRateMultiplier;

    // CHEST CHANCES
    public static float BigChestChance;
    public static void AddBigChestChance(float value) {

        // Clamp value to max 1 (100%)
        if (BigChestChance + value > 1) {
            BigChestChance = 1;
        } else {
            BigChestChance += value;
        }
    }

    public static float FireDamageMultiplier;
    public static float BaseFireDamage;

    [Header("OTHER VARS")]

    [SerializeField] private bool canSwitchWeapons = true;

    public bool iFrame;
    
    private Vector2 movementInput;

    // Current direction vector
    [SerializeField] private Vector2 currentDirection;

    // Dash duration boolean
    [SerializeField] private bool isDashing = false;

    // Dash cooldown boolean
    [SerializeField] private bool canDash = true;

    // Dash duration
    [SerializeField] private float dashingTime;
    private float dashTimer = 0;

    // Dash cooldown time
    [SerializeField] private float dashingCooldown;
    private float dashcdTimer = 0;

    // Dash force / power
    [SerializeField] private float dashingPower;

    [SerializeField] private float collisionOffset = 0.01f;

    [SerializeField] private float hurtShakeAmplitude;

    public bool savePressed = false;

    public bool ableToDrop = true;

    public int primaryWeaponIDTracker;
    public float damageModifierTracker;
    public float speedTracker;
    public float dodgeChanceTracker;
    public float moveSpeedMultTracker;
    public float viewRangeMultTracker;
    public float fireRateMultTracker;
    public float bigChestChanceTracker;
    public float baseFireDamageTracker;
    public float fireDamageMultTracker;

    private void OnEnable() {
        EOnDeath += StartDeath; // Subscribes wrapper function StartDeath to event OnDeath
        EOnEnergyFull += CallSound;
    }

    // Unsubscribes all events on player being disabled
    private void OnDisable() {
        EOnDeath -= StartDeath;
        EOnEnergyFull -= CallSound;
        EOnDodged = null;
        EOnDamaged = null;
        EOnEnergyFull = null;
        EOnMoving = null;
    }

    // Start is called before the first frame update
    public void PlayerStart(bool home)
    {

        // Ignores any collisions with enemies or bulletignore (layer 9 & 8)
        movementFilter.SetLayerMask(~(1 << 9 | 1 << 8));
        
        // Assigns necessary references
        FindReferences();
            
        // Makes sure player isn't in an iFrame starting off
        iFrame = false;

        // Load player
        string pathPlayer = Application.persistentDataPath + "/player.franny";

        // Load player info from saved game
        if (File.Exists(pathPlayer) && GameStateManager.SavePressed() == true) {
            LoadPlayer();
        } 
        // Save data exists but player did not click load save --> most likely a NextLevel() call
        else if (File.Exists(pathPlayer) && GameStateManager.SavePressed() == false) {
            Debug.Log("PLAYER SAVE DATA NEXT LEVEL CALL!");
        } 
        // Save data does not exist, and player clicked load save somehow
        else if (!File.Exists(pathPlayer) && GameStateManager.SavePressed() == true) {
            GameStateManager.SetSave(false);
            Debug.LogError("Saved player data not found while trying to load save. How did you get here?");
        } 
        // Save data does not exist and player did not click load save --> most likely started new game
        else if (!File.Exists(pathPlayer) && GameStateManager.SavePressed() == false) {

            // SET DEFAULT STATS
            SetDefaultStats();

            // Gives player default weapon if not inside Home or Tutorial
            if (!home && HomeManager.TutorialDone) {
                GameObject weaponObject = Instantiate(defaultWeapon.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                heldWeapons.Add(weaponObject);

                // Assigns correct starting ammo amounts
                if (heldWeapons[0].TryGetComponent<Weapon>(out var script)) {
                    PrimaryWeaponCurrentAmmo = script.ammoMax * AmmoMaxMultiplier;
                    script.currentAmmo = PrimaryWeaponCurrentAmmo;
                }
            }
        }

        // If player is not inside the Home—
        if (!home) {
            
            // Set health, energy, ammo, and coins UI references on each stage load
            healthBar = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<HealthBar>();
            energyBar = GameObject.FindGameObjectWithTag("EnergyBar").GetComponent<EnergyBar>();
            ammoBar = GameObject.FindGameObjectWithTag("AmmoBar").GetComponent<WeaponInfo>();
            coinsUI = GameObject.FindGameObjectWithTag("CoinsUI").GetComponent<CoinsUI>();

            // Set UI stat objects
            healthBar.SetMaxHealth(MaxHealth);
            healthBar.SetHealth(Health);
            energyBar.SetMaxEnergy(MaxEnergy);
            energyBar.SetEnergy(Experience);
            coinsUI.SetCoins(Coins);

            // If player isn't in the Tutorial (loaded up a saved run)—
            if (HomeManager.TutorialDone) {

                // Gives them their saved weapons and items back
                AddSavedWeapons();
                AddSavedItems();

                // Assigns held Weapon component
                if (weapon == null) {
                    weapon = GetComponentInChildren<Weapon>();
                    Debug.Log("WeaponFireMethod is null! Reassigned.");
                }
            }
        }
    }

    public void SetDefaultStats() {

        MaxHealth = 3;
        Health = MaxHealth;
        MaxEnergy = 20;
        Experience = 0;

        // Fire rate
        FireRateMultiplier = 1;

        // Set speed
        MoveSpeedMultiplier = 1;
        MoveSpeed = 3.2f;

        // Reset coins
        Coins = 0;

        // Reset item stats
        DodgeChance = 0;
        takenDamageMult = 1;

        DamageModifier = 1;

        FireDamageMultiplier = 1;
        BaseFireDamage = 1;

        // Set view range
        ViewRangeBase = 5;
        ViewRangeMultiplier = 1;

        // Chest chances
        BigChestChance = 0;

        // Reset saved weapons
        CurrentWeaponIndex = 0;
        PrimaryWeaponID = 1;
        PrimaryWeaponRarity = WeaponRarity.COMMON;
        SecondaryWeaponID = 0;
        SecondaryWeaponRarity = WeaponRarity.COMMON;

        AmmoMaxMultiplier = 1;

        HeldItemsCount = 0;
    }

    // Assigns all correct references
    private void FindReferences() {

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        if (rb == null) {
            rb = GetComponent<Rigidbody2D>();
            Debug.Log("PlayerController rb is null! Reassigned.");
        }
        if (animator == null) {
            animator = GetComponentInChildren<Animator>();
            Debug.Log("PlayerController animator is null! Reassigned.");
        }
        if (contactColl == null) {
            contactColl = GetComponent<CapsuleCollider2D>();
            Debug.Log("Collider2D contactColl is null! Reassigned.");
        }
        if (volumeProfile == null) {
            volumeProfile = FindAnyObjectByType<Volume>().sharedProfile;
            Debug.Log("VolumeProfile volumeProfile is null! Reassigned.");
        }
        if (saveIcon == null) {
            saveIcon = GameObject.FindGameObjectWithTag("SaveIcon");
            Debug.Log("Save icon is null! Reassigned.");
        }
        if (cameraLookAt == null) {
            cameraLookAt = GameObject.FindGameObjectWithTag("CameraLookAt").GetComponent<CameraLookAt>();
            Debug.Log("Camera look at component is null! Reassigned.");
        }
    }

    // Adds saved weapons if loading a previous run
    public void AddSavedWeapons() {

        // Add Thornbloom if ID matches and player is not currently holding weapons
        if (PrimaryWeaponID == 1 && heldWeapons.Count == 0) {
            GameObject weaponObject = Instantiate(defaultWeapon.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
            heldWeapons.Add(weaponObject);
        }

        // Add specified ID-matched weapon as primary weapon
        else if (PrimaryWeaponID != 1 && PrimaryWeaponID != 0) {

            WeaponPair pair;
            GameObject weaponObject;
            
            switch (PrimaryWeaponRarity) {

                case WeaponRarity.COMMON:
                    pair = lootList.seenCommonWeapons.Find(x => x.pickupScript.weaponID == PrimaryWeaponID);
                    weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                    heldWeapons.Add(weaponObject);
                    break;
                case WeaponRarity.UNCOMMON:
                    pair = lootList.seenUncommonWeapons.Find(x => x.pickupScript.weaponID == PrimaryWeaponID);
                    weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                    heldWeapons.Add(weaponObject);
                    break;
                case WeaponRarity.RARE:
                    pair = lootList.seenRareWeapons.Find(x => x.pickupScript.weaponID == PrimaryWeaponID);
                    weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                    heldWeapons.Add(weaponObject);
                    break;
                case WeaponRarity.EPIC:
                    pair = lootList.seenEpicWeapons.Find(x => x.pickupScript.weaponID == PrimaryWeaponID);
                    weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                    heldWeapons.Add(weaponObject);
                    break;
                case WeaponRarity.LEGENDARY:
                    pair = lootList.seenLegendaryWeapons.Find(x => x.pickupScript.weaponID == PrimaryWeaponID);
                    weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                    heldWeapons.Add(weaponObject);
                    break;
            }
        }

        // Add saved secondary weapon
        if (SecondaryWeaponID != 0) {

            WeaponPair pair;
            GameObject weaponObject;

            if (SecondaryWeaponID == 1) {
                pair = defaultWeapon;
                weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                heldWeapons.Add(weaponObject);
            } else {
                switch (SecondaryWeaponRarity) {

                    case WeaponRarity.COMMON:
                        pair = lootList.seenCommonWeapons.Find(x => x.pickupScript.weaponID == SecondaryWeaponID);
                        Debug.Log(pair);
                        Debug.Log(pair.pickupScript.objectToSpawn);
                        weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                        heldWeapons.Add(weaponObject);
                        break;
                    case WeaponRarity.UNCOMMON:
                        pair = lootList.seenUncommonWeapons.Find(x => x.pickupScript.weaponID == SecondaryWeaponID);
                        weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                        heldWeapons.Add(weaponObject);
                        break;
                    case WeaponRarity.RARE:
                        pair = lootList.seenRareWeapons.Find(x => x.pickupScript.weaponID == SecondaryWeaponID);
                        weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                        heldWeapons.Add(weaponObject);
                        break;
                    case WeaponRarity.EPIC:
                        pair = lootList.seenEpicWeapons.Find(x => x.pickupScript.weaponID == SecondaryWeaponID);
                        weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                        heldWeapons.Add(weaponObject);
                        break;
                    case WeaponRarity.LEGENDARY:
                        pair = lootList.seenLegendaryWeapons.Find(x => x.pickupScript.weaponID == SecondaryWeaponID);
                        weaponObject = Instantiate(pair.pickupScript.objectToSpawn, weaponPivot.transform.position, Quaternion.identity, weaponPivot.transform);
                        heldWeapons.Add(weaponObject);
                        break;
                }
            }
        }

        // Update weapons with saved ammo amounts
        if (heldWeapons.Count == 2) {
            if (heldWeapons[0].TryGetComponent<Weapon>(out var primary)) {
                primary.currentAmmo = PrimaryWeaponCurrentAmmo;
                primary.ammoMax *= AmmoMaxMultiplier;
            }

            if (heldWeapons[1].TryGetComponent<Weapon>(out var secondary)) {
                secondary.currentAmmo = SecondaryWeaponCurrentAmmo;
                secondary.ammoMax *= AmmoMaxMultiplier;
            }
        } else if (heldWeapons.Count > 0) {
            if (heldWeapons[0].TryGetComponent<Weapon>(out var primary)) {
                primary.currentAmmo = PrimaryWeaponCurrentAmmo;
                primary.ammoMax *= AmmoMaxMultiplier;
            }
        }

        // Set held weapon to previously held weapon upon entering new level
        if (CurrentWeaponIndex == 0 && heldWeapons.Count > 0) {
            if (heldWeapons[0].TryGetComponent<Weapon>(out var weaponScript)) {

                if (heldWeapons.Count > 1) {
                    heldWeapons[1].SetActive(false);
                }

                if (weapon == null) {
                    weapon = weaponScript;
                    CurrentWeaponIndex = 0;
                }

                // Sets maximum weapon ammo UI stat to base value * static multiplier
                ammoBar.SetMaxAmmo(weaponScript.ammoMax * AmmoMaxMultiplier);
                ammoBar.SetAmmo(PrimaryWeaponCurrentAmmo, weaponScript);
                ammoBar.weaponSprite.sprite = weaponScript.spriteRenderer.sprite;
            }
        } else if (CurrentWeaponIndex == 1 && heldWeapons.Count > 1) {
            if (heldWeapons[1].TryGetComponent<Weapon>(out var weaponScript)) {

                heldWeapons[0].SetActive(false);

                if (weapon == null) {
                    weapon = weaponScript;
                    CurrentWeaponIndex = 1;
                }

                // Sets maximum weapon ammo to base value * static multiplier
                ammoBar.SetMaxAmmo(weaponScript.ammoMax * AmmoMaxMultiplier);
                ammoBar.SetAmmo(SecondaryWeaponCurrentAmmo, weaponScript);
                ammoBar.weaponSprite.sprite = weaponScript.spriteRenderer.sprite;
            }
        }
    }

    // Adds saved items if loading a previous run
    public void AddSavedItems() {

        // For every saved item needing to be added—
        for (int i = 0; i < HeldItemsCount; i++) {

            ItemPickup item;
            GameObject itemObject;
            
            switch (HeldItemsRarity[i]) {
                case ItemRarity.COMMON:

                    // Finds item in total items list
                    item = itemList.seenCommonItems.Find(x => x.itemID == HeldItemsID[i]);

                    // Spawns it in and immediately hides it
                    itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                    itemObject.SetActive(false);

                    // Adds item to player's active items
                    heldItems.Add(itemObject.GetComponent<ItemPickup>());

                    // Triggers any OnPickup abilities
                    if (heldItems[i].type == ItemType.ABILITY) {
                        item.effect.OnPickup();
                    }
                    break;
                case ItemRarity.UNCOMMON:

                    // Same as above ^^
                    item = itemList.seenUncommonItems.Find(x => x.itemID == HeldItemsID[i]);
                    itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                    itemObject.SetActive(false);
                    heldItems.Add(itemObject.GetComponent<ItemPickup>());
                    if (heldItems[i].type == ItemType.ABILITY) {
                        item.effect.OnPickup();
                    }
                    break;
                case ItemRarity.RARE:

                    // Same as above ^^
                    item = itemList.seenRareItems.Find(x => x.itemID == HeldItemsID[i]);
                    itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                    itemObject.SetActive(false);
                    heldItems.Add(itemObject.GetComponent<ItemPickup>());
                    if (heldItems[i].type == ItemType.ABILITY) {
                        item.effect.OnPickup();
                    }
                    break;
                case ItemRarity.EPIC:

                    // Same as above ^^
                    item = itemList.seenEpicItems.Find(x => x.itemID == HeldItemsID[i]);
                    itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                    itemObject.SetActive(false);
                    heldItems.Add(itemObject.GetComponent<ItemPickup>());
                    if (heldItems[i].type == ItemType.ABILITY) {
                        item.effect.OnPickup();
                    }
                    break;
                case ItemRarity.LEGENDARY:

                    // Same as above ^^
                    item = itemList.seenLegendaryItems.Find(x => x.itemID == HeldItemsID[i]);
                    itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                    Debug.Log(item.gameObject);
                    itemObject.SetActive(false);
                    heldItems.Add(itemObject.GetComponent<ItemPickup>());
                    if (heldItems[i].type == ItemType.ABILITY) {
                        item.effect.OnPickup();
                    }
                    break;
                case ItemRarity.SPECIAL:

                    // Same as above ^^
                    item = itemList.seenSpecialItems.Find(x => x.itemID == HeldItemsID[i]);
                    itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                    itemObject.SetActive(false);
                    heldItems.Add(itemObject.GetComponent<ItemPickup>());
                    if (heldItems[i].type == ItemType.ABILITY) {
                        item.effect.OnPickup();
                    }
                    break;
                case ItemRarity.FLOWER:

                    // Same as above ^^
                    item = itemList.seenFlowerItems.Find(x => x.itemID == HeldItemsID[i]);
                    itemObject = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                    itemObject.SetActive(false);
                    heldItems.Add(itemObject.GetComponent<ItemPickup>());
                    if (heldItems[i].type == ItemType.ABILITY) {
                        item.effect.OnPickup();
                    }
                    break;
            }
        }
    }

    private void Update() {

        damageModifierTracker = DamageModifier;
        speedTracker = MoveSpeed;
        dodgeChanceTracker = DodgeChance;
        moveSpeedMultTracker = MoveSpeedMultiplier;
        viewRangeMultTracker = ViewRangeMultiplier;
        fireRateMultTracker = FireRateMultiplier;
        bigChestChanceTracker = BigChestChance;
        primaryWeaponIDTracker = PrimaryWeaponID;

        // If map was loaded and saved, save player as well
        if (savePressed) {
            savePressed = false;
            PlayerStart(false);
            SavePlayer();
            GameStateManager.SetSave(false);
        }

        // If in a menu, then do not take any input
        if (GameStateManager.GetState() != GAMESTATE.PLAYING) {
            return;
        }

        // Dash
        /* if (Input.GetKeyDown(KeyCode.Space) && canDash) {
            canDash = false;
            isDashing = true;
            animator.SetBool("Dash", true);
        } */

        // WEAPON SWITCHING AND DROPPING MECHANICS

        // Switch to first weapon
        if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && canSwitchWeapons && heldWeapons.Count == 2) {

            if (!heldWeapons[0].activeInHierarchy) {
                StartWeaponSwitch(0, CurrentWeaponIndex);
                GameStateManager.EOnWeaponSwitch?.Invoke();
            }
        }
        // Switch to second weapon
        else if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && canSwitchWeapons && heldWeapons.Count == 2) {

            if (!heldWeapons[1].activeInHierarchy) {
                StartWeaponSwitch(1, CurrentWeaponIndex);
                GameStateManager.EOnWeaponSwitch?.Invoke();
            }
        } else if (Input.GetMouseButtonDown(1) && canSwitchWeapons && heldWeapons.Count == 2) {

            if (!heldWeapons[0].activeInHierarchy) {
                StartWeaponSwitch(0, CurrentWeaponIndex);
                GameStateManager.EOnWeaponSwitch?.Invoke();
            } else if (!heldWeapons[1].activeInHierarchy) {
                StartWeaponSwitch(1, CurrentWeaponIndex);
                GameStateManager.EOnWeaponSwitch?.Invoke();
            }
        }

        // Drop selected weapon
        if (Input.GetKeyDown(KeyCode.Q) && weapon != null && ableToDrop) {
            InitiateDrop(weapon, false);
        }
    }

    public void InitiateDrop(Weapon dropping, bool replace) {
        StartCoroutine(DropWeapon(dropping, replace));
    }

    // Drop currently held weapon
    public IEnumerator DropWeapon(Weapon dropping, bool replace) {
        dropping.bursting = false;
        ableToDrop = false;

        // Put currently selected weapon's pickup object into temporary variable
        GameObject dropped = Instantiate(dropping.weaponPickup, transform.position, Quaternion.identity);

        weapon = null;

        // If script is obtainable, set this weapon to the pickup object's weaponObject (to be parented to player on pickup)
        if (dropped.TryGetComponent<WeaponPickup>(out var script)) {

            // Set weapon pickup to "dropped" setting
            script.dropped = true;

            Debug.Log(CurrentWeaponIndex);

            // Drop a new pickup weapon
            script.objectToSpawn = heldWeapons[CurrentWeaponIndex];

            // Delete dropped weapon
            if (heldWeapons.Count == 2) {
                int queuedForDeletion = CurrentWeaponIndex;

                // If not replacing an existing weapon—
                if (!replace) {

                    // Switch to other weapon before deleting
                    StartWeaponSwitch(heldWeapons.Count - 1 - CurrentWeaponIndex, CurrentWeaponIndex);

                    // Unparent and disable dropped weapon
                    heldWeapons[queuedForDeletion].transform.parent = null;
                    heldWeapons[queuedForDeletion].SetActive(false);

                    // Remove dropped weapon from held weapons
                    heldWeapons.Remove(heldWeapons[queuedForDeletion]);
                } 
                // If replacing an existing weapon—
                else {

                    // Unparent and disable dropped weapon
                    heldWeapons[queuedForDeletion].transform.parent = null;
                    heldWeapons[queuedForDeletion].SetActive(false);
                }

            } else {
                int queuedForDeletion = CurrentWeaponIndex;

                // Unparent and disable dropped weapon
                heldWeapons[queuedForDeletion].transform.parent = null;
                heldWeapons[queuedForDeletion].SetActive(false);

                // Remove dropped weapon from held weapons
                heldWeapons.Remove(heldWeapons[queuedForDeletion]);
                dropping = null;
            }
        } else {
            Debug.LogWarning("Could not find WeaponPickup component on weapon being dropped!");
        }

        switch (heldWeapons.Count) {

            // If no weapons are held, set everything to null and disable ammo UI bar
            case 0:
                ammoBar.gameObject.SetActive(false);

                CurrentWeaponIndex = 0;

                PrimaryWeaponID = 0;
                PrimaryWeaponRarity = WeaponRarity.COMMON;

                SecondaryWeaponID = 0;
                SecondaryWeaponRarity = WeaponRarity.COMMON;
                break;
            // If one held weapon left after dropping, set primary vars to that weapon, and null secondary vars
            case 1:
                CurrentWeaponIndex = 0;

                PrimaryWeaponID = weapon.id;
                PrimaryWeaponRarity = weapon.rarity;

                SecondaryWeaponID = 0;
                SecondaryWeaponRarity = WeaponRarity.COMMON;
                break;
            case 2:
                if (heldWeapons[0].TryGetComponent<Weapon>(out var primary)) {
                    PrimaryWeaponID = primary.id;
                    PrimaryWeaponRarity = primary.rarity;
                }
                if (heldWeapons[1].TryGetComponent<Weapon>(out var secondary)) {
                    SecondaryWeaponID = secondary.id;
                    SecondaryWeaponRarity = secondary.rarity;
                }
                break;
        }

        GameStateManager.EOnWeaponDrop?.Invoke();

        yield return new WaitForSeconds(0.6f);

        ableToDrop = true;
    }

    public void StartWeaponSwitch(int switchTo, int switchFrom) {
        StartCoroutine(SwitchWeapons(switchTo, switchFrom));
    }

    public IEnumerator SwitchWeapons(int switchTo, int switchFrom) {

        // Prevent switching weapons while already switching weapons
        canSwitchWeapons = false;

        // Disable old weapon if switching between two different weapons
        if (switchTo != switchFrom) {

            // Reset old weapon's ability to fire if mid-way through firing
            if (heldWeapons[switchTo].TryGetComponent<Weapon>(out var oldWeapon)) {
                oldWeapon.bursting = false;
            }

            // Disable old weapon
            heldWeapons[switchFrom].SetActive(false);
        }

        if (heldWeapons[switchTo].TryGetComponent<Weapon>(out var gotWeapon)) {

            // Set weapon script reference to new weapon
            weapon = gotWeapon;

            // Set currently held weapon index number to new weapon
            CurrentWeaponIndex = switchTo;

            // Enable new weapon
            if (!heldWeapons[switchTo].activeInHierarchy) {
                heldWeapons[switchTo].SetActive(true);
            }

            // If ammo UI bar is disabled, enable it
            if (!ammoBar.gameObject.activeInHierarchy) {
                ammoBar.gameObject.SetActive(true);
            }

            ammoBar.SetMaxAmmo(gotWeapon.ammoMax * AmmoMaxMultiplier);

            switch (switchTo) {
                case 0:
                    ammoBar.SetAmmo(PrimaryWeaponCurrentAmmo, gotWeapon);
                    break;
                case 1:
                    ammoBar.SetAmmo(SecondaryWeaponCurrentAmmo, gotWeapon);
                    break;
            }

            ammoBar.weaponSprite.sprite = gotWeapon.spriteRenderer.sprite;
        }
        else {
            Debug.LogError("Tried to switch to nonexistent weapon!");
        }

        yield return new WaitForSeconds(0.6f);

        canSwitchWeapons = true;
    }
    
    private void FixedUpdate() {

        // If in a menu, then do not take any input
        if (GameStateManager.GetState() != GAMESTATE.PLAYING) {
            animator.SetBool("IsMoving", false);
            return;
        }

        // Dash for the remaining duration, and don't take anything else as input
        if (isDashing) {

            // Enable reflection bullet radius
            dashColl.gameObject.SetActive(true);

            // Reset velocity to zero before dashing
            rb.linearVelocity = Vector2.zero;
            TryDash(currentDirection);
            
            // Dash duration timer
            dashTimer += Time.fixedDeltaTime;
                
            if(dashTimer > dashingTime) {

                // Disable reflection bullet radius
                dashColl.gameObject.SetActive(false);

                // End dash
                isDashing = false;

                // End dash animation
                animator.SetBool("Dash", false);

                // Reser velocity to zero after dashing
                rb.linearVelocity = Vector2.zero;

                // Reset dash duration timer
                dashTimer = 0;
            }

            return;
        }

        // Movement system if you're not dead lol
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {

            // Dash cooldown timer
            if (!isDashing && !canDash) {
                dashcdTimer += Time.fixedDeltaTime;

                if(dashcdTimer > dashingCooldown) {

                    // Reset dash cooldown
                    canDash = true;

                    // Reset dash cooldown timer
                    dashcdTimer = 0;
                }
            }

            if (movementInput != Vector2.zero) {
                bool success = TryMove(movementInput);
                currentDirection = movementInput;

                if (!success) {
                    success = TryMove(new Vector2(movementInput.x, 0));
                    currentDirection = new Vector2(movementInput.x, 0);

                    if (!success) {
                        success = TryMove(new Vector2(0, movementInput.y));
                        currentDirection = new Vector2(0, movementInput.y);
                    }
                }

                animator.SetBool("IsMoving", success);
            } else {
                animator.SetBool("IsMoving", false);
            }

            // Set direction of sprite to movement direction
            if(movementInput.x < 0) {
                spriteRenderer.flipX = true;
            } else if (movementInput.x > 0) {
                spriteRenderer.flipX = false;
            }
        }
    }

    // Dash function
    private bool TryDash(Vector2 direction) {

        if (direction != Vector2.zero) {

            int count = rb.Cast(
                direction, 
                movementFilter, 
                castCollisions, 
                MoveSpeed * Time.fixedDeltaTime + collisionOffset);
        
            if(count == 0) {
                rb.AddForce(direction * dashingPower * Time.fixedDeltaTime, ForceMode2D.Impulse);
                return true;
            } else {
                return false;
            }
        } else {
            // can't move if there's no direction to move in
            return false;
        }
    }

    // Move function
    private bool TryMove(Vector2 direction) {
        if (direction != Vector2.zero) {
            int count = rb.Cast(direction, movementFilter, castCollisions, MoveSpeed * Time.fixedDeltaTime + collisionOffset);
        
            if(count == 0) {
                EOnMoving?.Invoke();
                rb.MovePosition(rb.position + direction * MoveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
        } else {
            // can't move if there's no direction to move in
            return false;
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        /*
        animator.SetTrigger("MeleeAttack");
        */
    }

    public void TakeDamage(int damage) {

        if (GameStateManager.GetState() == GAMESTATE.PLAYING && !iFrame) {
            iFrame = true;

            if (DodgeChance != 0) {
                float rand = UnityEngine.Random.value;

                if (rand <= DodgeChance) {
                    EOnDodged?.Invoke();
                } else {
                    Damage(damage);
                }

            } else {
                Damage(damage);
            }
        }
    }

    public void Damage(int damage) {
        EOnDamaged?.Invoke(); // Triggers the OnDamaged event

        iFrame = true;
        StartCoroutine(SetHurtFlash(true));
        TriggerCamShake();
        Health -= Mathf.RoundToInt(damage * takenDamageMult);
        healthBar.SetHealth(currentHealth);

        animator.SetBool("Hurt", true);
    }

    private IEnumerator SetHurtFlash(bool condition) {

        if (volumeProfile != null) {
            if (volumeProfile.TryGet<ColorAdjustments>(out var colorAdjust)) {
                colorAdjust.active = condition;
                yield return new WaitForSeconds(0.2f);
                colorAdjust.active = !condition;
            }
        }

        yield return null;
    }

    public void TriggerCamShake() {

        FindReferences();

        if (camShake == null) {
            camShake = GetComponent<CinemachineImpulseSource>();
            Debug.Log("CinemachineImpulseSource camShake is null! Reassigned.");
        }

        Vector3 mousePos = GameStateManager.ToWorldPoint(Input.mousePosition, mainCam);

        Vector3 direction = mousePos - transform.position;

        camShake.GenerateImpulse(direction.normalized * hurtShakeAmplitude);
    }

    public void CallSound() {
        AudioManager.instance.PlaySoundByName("call", transform);
    }

    public void SavePlayer () {
        SaveSystem.SavePlayer(this, weapon, saveIcon);
        Debug.Log("SAVE PLAYER CALLED");
    }

    public void LoadPlayer() {
        
        // Load save data
        PlayerData data = SaveSystem.LoadPlayer();

        // Load health
        MaxHealth = data.playerMaxHealth;
        Health = data.playerHealth;
        healthBar.SetMaxHealth(MaxHealth);
        healthBar.SetHealth(Health);

        // Load damage modifier
        DamageModifier = data.playerDamageModifier;

        // Load experience level
        MaxEnergy = data.maxExperienceLevel;
        Experience = data.experienceLevel;
        energyBar.SetMaxEnergy(data.maxExperienceLevel);
        energyBar.SetEnergy(data.experienceLevel);

        // Set speeds
        MoveSpeed = data.playerMoveSpeed;
        MoveSpeedMultiplier = data.moveSpeedMult;
        FireRateMultiplier = data.playerFireRateMult;

        // Load coins
        Coins = data.playerCoins;
        coinsUI.SetCoins(data.playerCoins);

        // Load weapons
        PrimaryWeaponID = data.primaryWeaponID;
        PrimaryWeaponRarity = (WeaponRarity)data.primaryWeaponRarity;
        SecondaryWeaponID = data.secondaryWeaponID;
        SecondaryWeaponRarity = (WeaponRarity)data.secondaryWeaponRarity;

        AmmoMaxMultiplier = data.ammoMaxMultiplier;
        PrimaryWeaponCurrentAmmo = data.primaryWeaponCurrentAmmo;
        SecondaryWeaponCurrentAmmo = data.secondaryWeaponCurrentAmmo;

        // Load items
        HeldItemsCount = data.heldItemsCount;
        HeldItemsID = new(data.heldItemsID);
        HeldItemsRarity = new(data.heldItemsRarities);

        // Load dodge chance
        DodgeChance = data.dodgeChance;
        takenDamageMult = data.takenDamageMult;

        // Load view range
        ViewRangeBase = data.viewRangeBase;
        ViewRangeMultiplier = data.viewRangeMult;

        BigChestChance = data.bigChestChance;

        FireDamageMultiplier = data.fireDamageMultiplier;
        BaseFireDamage = data.baseFireDamage;

        Debug.Log("LOADED PLAYER");
    }

    public void StartDeath() {
        StartCoroutine(PlayerDeath());
    }

    public IEnumerator PlayerDeath() {
        GameStateManager.SetState(GAMESTATE.GAMEOVER);
        
        animator.SetBool("Death", true);
        AudioManager.instance.PlaySoundByName("player_death", gameObject.transform);

        yield return new WaitForSeconds(1f);

        ResetRun();
        IncrementDeath();
    }

    public void ResetRun() {

        lootList.ResetAllWeapons();
        itemList.ResetAllItems();
        heldItems.Clear();
        HeldItemsID.Clear();
        HeldItemsRarity.Clear();
        HeldItemsCount = 0;
    }

    public void IncrementDeath() {

        string pathHome = Application.persistentDataPath + "/home.soni";
        string pathMap = Application.persistentDataPath + "/map.chris";
        string pathPlayer = Application.persistentDataPath + "/player.franny";

        HomeManager.PlayerDeaths++;
        
        // If home save file is found—
        if (File.Exists(pathHome)) {

            // Reset run progress stats
            GameStateManager.SetLevel(0);
            GameStateManager.SetStage(0);

            // Deletes current run stat files
            if (File.Exists(pathMap) && File.Exists(pathPlayer)) {
                File.Delete(pathMap);
                File.Delete(pathPlayer);
                RefreshEditorWindow();
            } else {
                Debug.LogWarning("Either player or map save data file not found!");
            }

            Debug.Log("DIED AND WENT HOME");

            // Loads Home scene
            TransitionManager.StartLeaf(0);
        } 
        // If home save file is NOT found—
        else {

            // Reset run progress stats
            GameStateManager.SetLevel(0);
            GameStateManager.SetStage(0);

            // Deletes current run stat files
            if (File.Exists(pathMap) && File.Exists(pathPlayer)) {
                File.Delete(pathMap);
                File.Delete(pathPlayer);
                RefreshEditorWindow();
            } else {
                Debug.LogWarning("Either player or map save data file not found!");
            }

            Debug.LogWarning("DIED AND COULD NOT FIND HOME SAVE FILE");

            // Loads Home scene
            TransitionManager.StartLeaf(0);
        }
    }

    private void RefreshEditorWindow() {

        #if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
		#endif

    }
}
