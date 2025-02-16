using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Game.Core.Rendering;

public enum EnemyType {
    CONTACT, RANGED, SPLITTER, STATIONARY, MINIBOSS, BOSS, DEAD
}

public abstract class Enemy : MonoBehaviour
{

    [Tooltip("This enemy's type.")]
    public EnemyType enemyType;

    [Header("SCRIPT REFERENCES")]

    [Tooltip("This enemy's animator component.")]
    public Animator animator;

    [Tooltip("This enemy's level map reference. (Assigned at runtime)")]
    public WalkerGenerator map;

    [Tooltip("This enemy's current target.")]
    protected Vector3 target;

    [Tooltip("Reference to the active player. (Assigned at runtime)")]
    public PlayerController player;

    [Tooltip("This enemy's pathfinder script.")]
    [SerializeField] private Seeker seeker;

    [Tooltip("This enemy's follow collider, responsible for how far away it will follow a target.")]
    [SerializeField] protected CircleCollider2D followCollider;

    [Tooltip("This enemy's contact collider, responsible for how far contact enemies can attack from (NOT EQUAL TO HITBOX). Should only be set for Contact enemies.")]
    public Collider2D contactColl;

    [Tooltip("This enemy's Rigidbody2D component.")]
    public Rigidbody2D rb;

    [Tooltip("This enemy's hitbox.")]
    public Collider2D hitbox;

    [Tooltip("This enemy's SpriteRenderer.")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("This enemy's SpriteRenderer that is responsible for drawing the line of fire.")]
    public SpriteRenderer lineSpriteRenderer;

    [Tooltip("This enemy's LineRenderer2D component for drawing line of fire.")]
    public LineRenderer2D lineRenderer;

    [Tooltip("List of this enemy's possible weapon drops.")]
    [SerializeField] private List<WeaponPair> weaponDropsList;

    [Tooltip("List of this enemy's possible weapon drops.")]
    [SerializeField] private List<ItemPickup> itemDropsList;

    [Tooltip("A reference to the bronze coin prefab for spawning.")]
    [SerializeField] private GameObject coinBronze;

    [Tooltip("A reference to the silver coin prefab for spawning.")]
    [SerializeField] private GameObject coinSilver;

    [Tooltip("A reference to the gold coin prefab for spawning.")]
    [SerializeField] private GameObject coinGold;

    [Tooltip("The minimum possible amount of coins this enemy drops upon death.")]
    [SerializeField] private int minCoins;

    [Tooltip("The maximum possible amount of coins this enemy drops upon death.")]
    [SerializeField] private int maxCoins;

    [Tooltip("The experience orb GameObject this enemy drops upon death.")]
    [SerializeField] private GameObject expOrb;

    // Minimum and maximum experience / energy to drop on death
    [SerializeField] private int minExp;
    [SerializeField] private int maxExp;

    [Tooltip("All contact attacks this enemy has—SUM OF THESE attackChance VARIABLES MUST BE EQUAL TO 1.")]
    [SerializeField] protected List<EnemyAttackStruct> contactAttacks;

    [Tooltip("All ranged attacks this enemy has—SUM OF THESE attackChance VARIABLES MUST BE EQUAL TO 1.")]
    [SerializeField] protected List<EnemyAttackStruct> rangedAttacks;

    public EnemyAttackBase currentAttack;

    [Space(10)]
    [Header("ENEMY STATS")]

    [Tooltip("The variable responsible for how much the spawn rate curve is vertically amplified. - (a)")]
    public float spawnChanceVertAmp = 1;

    [Tooltip("The variable responsible for how steep the spawn rate curve is across levels. - (m)")]
    public float spawnChanceMultiplier = 1;

    [Tooltip("The variable responsible for horizontal transformation of the spawn rate curve. - (h)")]
    public float spawnChanceXTransform = 1;

    [Tooltip("The variable responsible for the exponential amplification of the spawn rate curve. - (p)")]
    public float spawnChanceExponent = 2;

    [Tooltip("The variable responsible for vertical transformation of the spawn rate curve. - (v)")]
    public float spawnChanceYTransform = 0;

    [Tooltip("Maximum overall target chance for this enemy to spawn in any level.")]
    public float maxSpawnChance = 1;

    [Tooltip("The minimum target amount of this enemy to spawn.")]
    public int minSpawnCount;

    [Tooltip("The maximum target amount of this enemy to spawn.")]
    public int maxSpawnCount;

    [Tooltip("The speed that this enemy moves when it is chasing a target.")]
    [SerializeField] protected float chaseSpeed;

    [Tooltip("The speed that this enemy moves when it is wandering around.")]
    [SerializeField] protected float wanderSpeed;

    // This enemy's attack speed
    public float rangedAttackCooldownMin;
    public float rangedAttackCooldownMax;
    public float attackCooldown;

    [Tooltip("Time it takes for the enemy to charge a shot.")]
    public float chargeTime;

    // Boolean to determine whether attack animation is playing
    protected bool attackAnim;

    public bool seen;

    public bool kbEd;

    public bool canFire;

    public bool seesPlayer = false;

    public bool inContactColl;

    public bool charging = false;
    public bool attacking = false;

    [Space(10)]
    [Header("PATHFINDING")]

    // This enemy's pathfinding waypoint distance
    [SerializeField] private float nextWaypointDistance = 3f;

    Pathfinding.Path path;
    private int currentWaypoint = 0;
    protected bool reachedEndOfPath = false;
    public bool inFollowRadius;
    protected Vector2 direction;
    protected int direc;
    protected Vector2 force;

    [SerializeField]
    protected bool canWander;

    protected float attackTimer;
    protected float wanderTimer = 0;
    protected float moveTime;
    protected float waitTime;
    protected float waitTimer = 0;

    protected bool timerSet = false;

    protected bool tileGot = false;

    protected bool expSpawn;
    protected bool coinSpawn;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (enemyType != EnemyType.DEAD && GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            SetEnemyType();

            if (rb == null) {
                rb = GetComponent<Rigidbody2D>();
                Debug.Log("ContactEnemy rb is null! Reassigned.");
            }
            if (hitbox == null) {
                hitbox = GetComponentInChildren<Collider2D>();
                Debug.Log("Collider2D hitbox is null! Reassigned.");
            }
            hitbox.enabled = true;

            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            attackAnim = false;
            expSpawn = false;
            coinSpawn = false;
            canFire = false;

            if (enemyType != EnemyType.STATIONARY) {
                if (seeker == null) {
                    Debug.Log("ContactEnemy seeker is null! Reassigned.");
                    seeker = GetComponent<Seeker>();
                }
                canWander = true;
                kbEd = false;

                InvokeRepeating(nameof(UpdatePath), 0f, .5f);
            }
        }
    }

    public virtual void SetEnemyType() {
        enemyType = EnemyType.CONTACT;
    }

    void UpdatePath() {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, target, OnPathComplete);
        }
    }

    // Attack cooldown timer
    public virtual void Cooldown()
    {
        if (!canFire) {
            attackTimer += Time.fixedDeltaTime;

            if (attackTimer > attackCooldown) {
                canFire = true;
                attackTimer = 0;
            }
        }
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING && enemyType != EnemyType.DEAD) {

            if (canWander && timerSet) {
                wanderTimer += Time.fixedDeltaTime;
                //Debug.Log("wanderTimer: " + wanderTimer);
                if(wanderTimer > moveTime) {
                    //Debug.Log("Done With WanderTimer");
                    canWander = false;
                    tileGot = false;
                    wanderTimer = 0;
                }
            }
            
            if (!canWander) {
                waitTimer += Time.fixedDeltaTime;
                //Debug.Log("waitTimer: " + waitTimer);
                if(waitTimer > waitTime) {
                    canWander = true;
                    timerSet = false;
                    waitTimer = 0;
                }
            }

            // Pathfinding
            Pathfinder();

            DirectionFacing();

            // Attack cooldown
            Cooldown();

            // If the enemy has any attacks—
            if (contactAttacks.Count > 0 || rangedAttacks.Count > 0) {

                // If enemy can fire and has seen the player and is not currently attacking or charging an attack—
                if (canFire && seen && seesPlayer && !charging && !attacking) {

                    // Roll attacks
                    RollAttacks();
                }
            }
        }
    }

    // Generate enemy attacks
    public virtual void RollAttacks() {

        List<EnemyAttackStruct> targetList = null;

        // If player is inside contact radius, uses a contact attack, otherwise uses a ranged attack
        switch (inContactColl) {

            case true:
                targetList = contactAttacks.Count > 0 ? contactAttacks : rangedAttacks;
                break;
            case false:
                targetList = rangedAttacks.Count > 0 ? rangedAttacks : contactAttacks;
                break;
        }

        if (targetList == null || targetList.Count <= 0) {
            Debug.LogError("Enemy attack TargetList is null or empty! " + gameObject.name);
            return;
        }

        bool attackSuccess = false;

        int counter = 0;

        while (!attackSuccess) {

            // Ensures that this loop stops at 5 maximum iterations
            // Uses first contact attack if this somehow occurs.
            if (counter > 4) {
                currentAttack = contactAttacks[0].attack;
                currentAttack.FiringMethod();

                attackSuccess = true;
                break;
            }

            // For every possible contact attack this enemy has—
            foreach (var attackStruct in targetList) {

                // Roll for spawning success
                float roll = UnityEngine.Random.value;

                // If the attack's success roll is successful—
                if (roll <= attackStruct.attackChance) {

                    // Sets the current attack and uses the attack
                    currentAttack = attackStruct.attack;
                    attackStruct.attack.FiringMethod();

                    // Don't use any other attack
                    attackSuccess = true;
                    break;
                }
            }

            counter++;
        }
    }

    // Ends the attack animation and begins actual attack (RUNS AT THE LAST FRAME OF ANIMATION)
    public virtual void BeginAttack() {

        if (currentAttack) {
            currentAttack.SpawnAttack();
        } else {
            Debug.LogError(this.name + " tried to attack without having a current attack!");
        }
    }

    public virtual void EndAttack() {
        animator.SetBool("Attack", false);
    }

    // Called at the end of the attack animation (after attack has been called mid-way through)
    public virtual void ResetOrbitalAttackAnim() {
        animator.SetBool("OrbitalAttack", false);
    }

    // PATHFINDER MOVEMENT and calling PlayerCheck()
    public virtual void Pathfinder() {
        // 1.
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;

        } else {
            reachedEndOfPath = false;
        }

        // 2.
        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;

        // 3.
        PlayerCheck();

        // 4.
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }
    }

    public virtual void PlayerCheck() {

        if (!timerSet) {

            // Sets the amount of time spent moving
            moveTime = UnityEngine.Random.Range(3, 5);

            // Sets a cooldown before wandering again
            waitTime = UnityEngine.Random.Range(5, 7);
            
            timerSet = true;
        }

        // If enemy is not currently taking knockback
        if (!kbEd) {

            // If player is in follow radius then chase
            if (inFollowRadius == true) {
                seen = true;
                canWander = false;
                force = Vector2.zero;
                target = player.transform.position;
                Chase();
            } 
            // If player is not in follow radius, and wander cooldown is reset, then wander
            else if (inFollowRadius == false && canWander) {

                // Gets target tile
                Vector3 randTile = GetWanderTile();

                // If tile hasn't been checked for validity
                if (!tileGot) {
                    tileGot = true;

                    // Set target to tile
                    target = randTile;
                }

                // Wander to tile
                Wander();
            }
        }
    }

    // Sprite direction facing
    public virtual void DirectionFacing() {

        if (!kbEd) {

            if (rb.linearVelocity.x >= 0.001f) {

                spriteRenderer.flipX = false;
                animator.SetBool("IsMoving", true);

            } else if (rb.linearVelocity.x <= -0.001f) {

                spriteRenderer.flipX = true;
                animator.SetBool("IsMoving", true);

            } else if (rb.linearVelocity.y <= -0.001 || rb.linearVelocity.y >= 0.001) {
                animator.SetBool("IsMoving", true);
            } else {
                animator.SetBool("IsMoving", false);
            }
        }
    }

    void OnPathComplete(Pathfinding.Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }

    public virtual void Chase() {

        // Sets direction and destination of path to Player
        force = chaseSpeed * Time.fixedDeltaTime * direction;

        // Moves towards target
        rb.AddForce(force);
    }

    public virtual void Wander() {        
        force = wanderSpeed * Time.fixedDeltaTime * direction;

        rb.AddForce(force);
    }

    public virtual Vector3 GetWanderTile() {

        // Picks a random tile within radius
        float tileX = UnityEngine.Random.Range(this.transform.position.x - followCollider.radius, 
            this.transform.position.y + followCollider.radius);

        float tileY = UnityEngine.Random.Range(this.transform.position.y - followCollider.radius, 
            this.transform.position.y + followCollider.radius);

        Vector3 tile = new(tileX, tileY);

        if (map.CheckGroundTile(tile)) {
            return tile;
        } else {
            GetWanderTile();
        }

        return Vector3.zero;
    }

    public UnityEngine.Object Create(UnityEngine.Object original, Vector3 position, Quaternion rotation, WalkerGenerator gen) {
        GameObject entity = Instantiate(original, position, rotation) as GameObject;
        
        if (entity.TryGetComponent<Enemy>(out var enemy)) {
            enemy.map = gen;
            return entity;
        } else if (entity.GetComponentInChildren<Enemy>()) {
            entity.GetComponentInChildren<Enemy>().map = gen;
            return entity;
        } else {
            Debug.LogError("Could not find Enemy script or extension of such on this Object.");
            return null;
        }
    }

    public virtual void SpawnExp() {

        int rand = UnityEngine.Random.Range(minExp, maxExp);

        for (int i = 0; i < rand; i++) {
            Create(expOrb, this.transform.position, Quaternion.identity, this.map);
        }
    }

    // ENCLOSING FUNCTION FOR ENEMY DROPS
    public virtual void SpawnDrops() {

        DropCoins();

        DropItems();
    }

    // Enemy's item drops
    public virtual void DropItems() {

        // WEAPONS
        foreach (var weaponDrop in weaponDropsList) {

            float rand = UnityEngine.Random.value;

            if (rand <= weaponDrop.dropChance) {

                // PUT DROP CODE HERE
                Debug.Log("Created pickup drop!");
            }
        }

        // ITEMS
        foreach (var itemDrop in itemDropsList) {

            float rand = UnityEngine.Random.value;

            if (rand <= itemDrop.dropChance) {

                // PUT DROP CODE HERE
                Debug.Log("Created pickup drop!");
            }
        }
    }

    // Enemy's coin drops
    public virtual void DropCoins() {

        int rand = UnityEngine.Random.Range(minCoins, maxCoins);

        Debug.Log("total coins:" + rand);

        // Separate coin values and incrementally drop from highest value to lowest value coins to meet total
        if (rand >= 20) {

            // Drop a single gold coin if total coins is exactly 20
            if (rand == 20) {
                Create(coinGold, this.transform.position, Quaternion.identity, this.map);
                return;
            }

            // Drop gold coins
            for (int g = 0; g < (rand - (rand % 20)) / 20; g++) {
                Create(coinGold, this.transform.position, Quaternion.identity, this.map);
            }

            // Update total amount of coins to exclude gold
            rand %= 20;

            // Drop silver coins
            for (int s = 0; s < (rand - (rand % 5)) / 5; s++) {
                Create(coinSilver, this.transform.position, Quaternion.identity, this.map);
            }

            // Drop bronze coins as leftover
            for (int b = 0; b < rand % 5; b++) {
                Create(coinBronze, this.transform.position, Quaternion.identity, this.map);
            }

        } else if (rand >= 5) {

            // Drop a single silver coin if total coins is exactly 5
            if (rand == 5) {
                Create(coinSilver, this.transform.position, Quaternion.identity, this.map);
                return;
            }

            // Drop silver coins
            for (int s = 0; s < (rand - (rand % 5)) / 5; s++) {
                Create(coinSilver, this.transform.position, Quaternion.identity, this.map);
            }

            // Drop bronze coins as leftover
            for (int b = 0; b < rand % 5; b++) {
                Create(coinBronze, this.transform.position, Quaternion.identity, this.map);
            }

        } else {

            // Drop bronze coins
            for (int b = 0; b < rand; b++) {
                Create(coinBronze, this.transform.position, Quaternion.identity, this.map);
            }
        }
    }

    public virtual void RemoveHitbox() {
        hitbox.enabled = false;
    }

    // Called at the START of this enemy's death animation
    public virtual void EnemyDeath() {

        // Sets enemy type to DEAD
        enemyType = EnemyType.DEAD;

        GameStateManager.EOnEnemyDeath?.Invoke();

        // Sets force to 0 so that the enemy doesn't just fly off
        force = 0 * Time.fixedDeltaTime * direction;

        // Spawns EXP
        SpawnExp();
        SpawnDrops();

        // Increments dead enemy counter
        WalkerGenerator.AddDeadEnemy();
        Debug.Log(WalkerGenerator.GetDeadEnemies() + "/" + WalkerGenerator.EnemyTotal);
    }

    // Called at the END of this enemy's death animation
    public virtual void PostEnemyDeath() {

        // Removes enemy
        Destroy(gameObject);
    }
}
