using UnityEngine;

public class BulletScript : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    protected Vector3 mousePos;
    protected Camera mainCam;

    [SerializeField] protected Rigidbody2D rb;

    public Animator animator;

    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected Vector3 direction;

    [SerializeField] protected Collider2D coll;

    protected Weapon weapon;

    [Space(10)]
    [Header("STATS")]

    public IgnoredCollisionsList ignoredCollisions;

    protected Vector3 previousPosition;

    [Tooltip("The speed at which this type of bullet fires at.")]
    [SerializeField] protected float force;

    protected Vector2 error;

    public float damage;

    public float knockback;

    [Tooltip("Lower values are more accurate— 0 fires in a straight line.")]
    [SerializeField] protected float accuracy;

    [SerializeField] protected bool isFire;

    protected bool madeContact;

    public virtual void OnEnable() {
        GameStateManager.EOnBulletHitWall += SoundHitWall;
    }

    public virtual void OnDisable() {
        DisconnectEvents();
    }

    public virtual void DisconnectEvents() {
        GameStateManager.EOnBulletHitWall -= SoundHitWall;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {

        previousPosition = transform.position;

        coll.enabled = true;

        madeContact = false;

        if (animator == null) {
            Debug.Log("BulletScript animator is null! Reassigned.");
            animator = GetComponent<Animator>();
        }
        if (rb == null) {
            Debug.Log("BulletScript rb is null! Reassigned.");
            rb = GetComponent<Rigidbody2D>();
        }

        mousePos = ToWorldPoint(Input.mousePosition);

        // DIRECTION OF THE BULLET

        direction = mousePos - weapon.transform.position;
        //Debug.Log(direction);

        if (direction == Vector3.zero) {
            direction = weapon.spawnPos.transform.position - weapon.transform.position;
        }

        // Determines the accuracy of the bullet (so bullets don't just fire in a straight line every time)
        error = UnityEngine.Random.insideUnitCircle * accuracy;

        // Sets the velocity and direction of the bullet which is acted on every frame from now on (this determines how the bullet moves)
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force + new Vector2(error.x, error.y);
        
        // Rotation of the bullet (which way it is facing, NOT which direction its moving in)
        Vector2 rot = rb.linearVelocity;
        if (rb.linearVelocity.x < 0) {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = true;
            
        } else {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }
        float angle = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Vector2 ToWorldPoint(Vector3 input) {

        Vector2 inCamera;
        Vector2 pixelAmount;
        Vector2 worldPoint;

        inCamera.y = mainCam.orthographicSize * 2;
        inCamera.x = inCamera.y * Screen.width / Screen.height;

        pixelAmount.x = Screen.width / inCamera.x;
        pixelAmount.y = Screen.height / inCamera.y;

        worldPoint.x = ((input.x / pixelAmount.x) - (inCamera.x / 2) + mainCam.transform.position.x);
        worldPoint.y = ((input.y / pixelAmount.y) - (inCamera.y / 2) + mainCam.transform.position.y);

        return worldPoint;
    }

    public void FixedUpdate() {

        // Sets the previous location of the bullet in FixedUpdate for more accurate (longer) raycasting
        previousPosition = transform.position;
    }

    public virtual void Update() {

        //transform.rotation.SetLookRotation(rb.linearVelocity);

        if (!madeContact) {
            //Debug.Log("Previous Position: " + previousPosition + "Current Position: " + rb.position);
            Debug.DrawLine(rb.position, previousPosition, Color.yellow, 0.1f);
            //rb.AddForce(direction.normalized * force * Time.fixedDeltaTime);
            //transform.Translate(0, error.y + force * Time.deltaTime, 0);
            //rb.MovePosition(transform.position + force * Time.deltaTime * direction);

            // Raycasts for any missed collisions between frames
            RaycastHit2D[] hits = Physics2D.RaycastAll(previousPosition, ((Vector3)rb.position - previousPosition).normalized, ((Vector3)rb.position - previousPosition).magnitude);

            // For all objects detected in the raycast—
            for (int i = 0; i < hits.Length; i++) {

                //Debug.Log(hits[i].collider.gameObject.layer + " " + hits[i].collider.transform.root.name);

                // If the object hit isn't supposed to be ignored, then try to deal damage and then destroy the bullet
                if (!ignoredCollisions.ignoredCollisions.Contains(hits[i].collider.gameObject.layer)) {
                    madeContact = true;
                    RegisterDamage(hits[i].collider.gameObject);
                    coll.enabled = false;
                    rb.linearVelocity = (Vector2)direction.normalized * 0;
                    animator.SetTrigger("Destroy");
                }
                
            }
        }
    }

    public UnityEngine.Object Create(UnityEngine.Object original, Vector3 position, Quaternion rotation, Weapon weapon, Camera cam) {
        GameObject bullet = Instantiate(original, position, rotation) as GameObject;
        
        if (bullet.TryGetComponent<BulletScript>(out var script)) {
            script.weapon = weapon;
            script.mainCam = cam;
            return bullet;
        } else if (bullet.GetComponentInChildren<BulletScript>()) {
            BulletScript bulletScript = bullet.GetComponentInChildren<BulletScript>();
            bulletScript.weapon = weapon;
            bulletScript.mainCam = cam;
            return bullet;
        } else {
            Debug.LogError("Could not find BulletScript script or extension of such on this Object.");
            return null;
        }
    }

    // Damage enemies or destroy self when hitting obstacles that aren't meant to be ignored
    public virtual void OnTriggerEnter2D(Collider2D other) {

        if (!madeContact) {
            if (!ignoredCollisions.ignoredCollisions.Contains(other.gameObject.layer)) {
                madeContact = true;
                RegisterDamage(other.gameObject);
                coll.enabled = false;
                rb.linearVelocity = (Vector2)direction.normalized * 0;
                animator.SetTrigger("Destroy");
            }
        }
       
    }

    public virtual void RegisterDamage(GameObject target) {

        //direction = mousePos - transform.position;

        // Checks whether collided object is an enemy
        if (target.CompareTag("Enemy")) {

            GameStateManager.EOnBulletHitEnemy?.Invoke(); // Triggers bullet hit enemy event

            // Tries to get the EnemyHealth component of collided enemy
            if (target.TryGetComponent<EnemyHealth>(out var enemy)) {

                // If bullet is a flame bullet, deal fire damage
                if (isFire && !enemy.immuneToFire) {
                    enemy.TakeFireDamage(damage, direction, knockback);
                }
                else {
                    enemy.TakeDamage(damage, direction, knockback);
                }
            }
            else {
                Debug.LogError("Could not get collided enemy's EnemyHealth component!");
            }
        } else {
            GameStateManager.EOnBulletHitWall?.Invoke(); // Triggers bullet hit wall (not enemy) event
        }
    }

    // Play non-enemy hit sound for this projectile // Subscribed to EOnBulletHitWall event
    public virtual void SoundHitWall() {
        AudioManager.instance.PlaySoundByName(weapon.bulletHitSound, transform);
    }

    public virtual void DestroyBullet() {
        DisconnectEvents();
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }
}
