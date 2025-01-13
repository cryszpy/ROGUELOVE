using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    protected Vector3 target;

    public Rigidbody2D rb;

    [SerializeField] protected Animator animator;

    [SerializeField] protected SpriteRenderer spriteRenderer;

    public Vector3 direction;

    [SerializeField] protected Collider2D coll;

    [Header("STATS")]

    [SerializeField] protected Vector2 spawnPoint;
    [SerializeField] protected GameObject weaponPivot;

    [SerializeField] protected float speed;

    [SerializeField] protected int damage = 2;

    [SerializeField] protected float knockback;

    [SerializeField] protected bool reflected = false;

    protected float timer = 0f;

    protected Vector2 error;

    [Tooltip("Lower values are more accurate— 0 fires in a straight line.")]
    [SerializeField] protected float accuracy;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER) {
            spawnPoint = new Vector2(transform.position.x, transform.position.y);

            coll.enabled = true;

            if (animator == null) {
                Debug.Log("BulletScript animator is null! Reassigned.");
                animator = GetComponent<Animator>();
            }

            if (rb == null) {
                Debug.Log("BulletScript rb is null! Reassigned.");
                rb = GetComponent<Rigidbody2D>();
            }

            target = GameObject.FindGameObjectWithTag("Player").transform.position;
            
            //direction = target - transform.position;

            // Determines the accuracy of the bullet (so bullets don't just fire in a straight line every time)
            error = UnityEngine.Random.insideUnitCircle * accuracy;

            // Sets the velocity and direction of the bullet which is acted on every frame from now on (this determines how the bullet moves)
            rb.linearVelocity = new Vector2(transform.right.x, transform.right.y).normalized * speed + new Vector2(error.x, error.y);

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
    }

    public virtual void Update() {
        if (GameStateManager.GetState() == GAMESTATE.GAMEOVER) {
            DestroyBullet();
        }
    }

    public UnityEngine.Object Create(UnityEngine.Object original, Vector3 position, Quaternion rotation, GameObject spawnPosition) {
        Debug.Log(spawnPosition);
        GameObject bullet = Instantiate(original, position, rotation) as GameObject;
        
        if (bullet.TryGetComponent<EnemyBulletScript>(out var script)) {
            script.weaponPivot = spawnPosition;
            //BulletStart(spawnPosition);
            return bullet;
        } else if (bullet.GetComponentInChildren<EnemyBulletScript>()) {
            bullet.GetComponentInChildren<EnemyBulletScript>().weaponPivot = spawnPosition;
            //BulletStart(spawnPosition);
            return bullet;
        } else {
            Debug.LogError("Could not find EnemyBulletScript script or extension of such on this Object.");
            return null;
        }
    }

    /*
    public virtual Vector2 Movement(float timer) {
        float x = timer * speed * transform.up.x;
        float y = timer * speed * transform.up.y;
        return new Vector2(x + spawnPoint.x + error.x, y + spawnPoint.y + error.y);
    } */

    // Damage player or destroy self when hitting obstacles
    public virtual void OnTriggerEnter2D(Collider2D other) {
        
        direction = target - transform.position;

        // Checks whether collided object is of layer Player
        if (other.gameObject.layer == 3) {

            // If not the reflect dash radius
            if (!other.CompareTag("PlayerDashRadius")) {

                // Deal damage to player if not null
                if (other != null) {
                                
                    if (other.TryGetComponent<PlayerController>(out var player)) {
                        player.TakeDamage(damage);
                    }

                    // Destroy bullet on contact with player
                    coll.enabled = false;
                    rb.linearVelocity = (Vector2)direction.normalized * 0;
                    animator.SetTrigger("Destroy");
                }
                else {
                    Debug.LogWarning("Enemy bullet collision is null!");
                }
            }
            // If collided object is the dash radius
            else {

                // Reflect bullet
                Vector2 temp = rb.linearVelocity;
                rb.linearVelocity = Vector2.zero;
                rb.linearVelocity = new Vector2(temp.x * -1, temp.y * -1);
                reflected = true;
            }
            
        }
        // If hitting anything other than the player (should destroy) or an enemy (should passthrough)
        // then destroy bullet
        else if (!other.CompareTag("Enemy")) {
            coll.enabled = false;
            rb.linearVelocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
        
        if (other.CompareTag("Enemy") && reflected) {
            if (other.TryGetComponent<EnemyHealth>(out var enemy)) {
                enemy.TakeDamage(damage, rb.linearVelocity, knockback);
            }

            // Destroy bullet on contact with enemy after reflection
            coll.enabled = false;
            rb.linearVelocity = (Vector2)direction.normalized * 0;
            animator.SetTrigger("Destroy");
        }
    }

    public virtual void DestroyBullet() {
        Destroy(gameObject);
    }
}
