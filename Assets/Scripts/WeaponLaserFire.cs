using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLaserFire : WeaponBurstFire
{

    [Header("LASER SCRIPT REFERENCES")] // -----------------------------------------------------------------------------------

    [SerializeField] protected LineRenderer lineRenderer;

    [SerializeField] private LayerMask layerMask;

    [Header("LASER STATS")] // -----------------------------------------------------------------------------------

    public float damage;

    public float knockback;

    public bool doesFireDamage;

    protected List<EnemyHealth> collidedEnemies = new();

    protected bool usingAmmo = false;
    protected float ammoTimer = 0;

    protected bool triggerSound = false;
    protected bool playingSound = false;

    [Header("LASER SHADER STATS")] // -----------------------------------------------------------------------------------

    [Range(2, 25)]
    [Tooltip("The amount of segments in the line.")]
    public int maxSegmentCount = 2;
    protected int segmentCount;

    [Range(0, 1.5f)]
    [Tooltip("The scale of randomness applied to every segment excluding start and end.")]
    public float variationScale = 0.5f;

    [Tooltip("Minimum possible length of line segments.")]
    public float minSegmentLength = 0.5f;

    [Tooltip("How fast segments move to their new positions if randomness is enabled.")]
    public float segmentMoveSpeed = 5f;

    [Tooltip("How often the segment randomization occurs. (s)")]
    public float randomizeTime;
    protected float randomizeTimer = 0;
    protected bool canRandomize = true;

    [Tooltip("The maximum distance a point can move before snapping to its desired location.")]
    public float elasticMax = 2;

    protected List<Vector3> positions;

    public override void Start()
    {
        base.Start();

        segmentCount = maxSegmentCount;
    }

    public override void FixedUpdate()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            
            Cooldown();

            // Firing logic, if not on cooldown and mouse button pressed, fire
            if (Input.GetMouseButton(0) && parent.currentAmmo > 0) {

                // Use ammo
                usingAmmo = true;

                // Raycasts laser
                parent.bursting = true;
                RaycastLaser();
            } else {
                triggerSound = false;
                UpdateVisual(false);
            }

            // Reset charging
            if (!Input.GetMouseButton(0) && chargeTimer > 0) {
                chargeTimer = 0;
                UpdateVisual(false);
            }
        }
    }

    public virtual void Update() {

        // If weapon does not have infinite ammo, use ammo while firing
        if (usingAmmo) {
            ammoTimer += Time.deltaTime;

            if (ammoTimer > parent.fireRate * PlayerController.FireRateMultiplier) {
                usingAmmo = false;
                ammoTimer = 0;

                UseAmmo();
            }
        }

        if (triggerSound && !playingSound) {
            playingSound = true;

            // Play firing sound
            if (!string.IsNullOrWhiteSpace(parent.fireSound)) {
                FireSound();
            }
        } else {

            // Stop sound
            playingSound = false;
        }

        // Line segment randomization timer
        if (!canRandomize) {
            randomizeTimer += Time.deltaTime;

            if (randomizeTimer > randomizeTime) {
                canRandomize = true;
                randomizeTimer = 0;
            }
        }

        if (lineRenderer.enabled == true && positions.Count > 0) {

            MoveSegments();
        }
    }

    // Weapon fire cooldown
    public override void Cooldown() {
        
        if (!canFire) {
            timer += Time.deltaTime;
            
            if(timer > parent.fireRate * PlayerController.FireRateMultiplier) {
                canFire = true;
                timer = 0;
            }
        }
    }

    public override void UseAmmo()
    {
        base.UseAmmo();
        
        // Start camera shake
        TriggerCamShake();
    }

    public virtual void RaycastLaser() {

        // Get mouse position
        Vector3 mousePos = GameStateManager.ToWorldPoint(Input.mousePosition, mainCam);
        Vector3 direction = mousePos - transform.position;

        // Raycast in the direction of mouse position
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100f, layerMask);

        // Update line renderer
        UpdateVisual(true, hit.point);

        // If raycast hits an enemy—
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy")) {

            if (hit.collider.gameObject.TryGetComponent<EnemyHealth>(out var script)) {

                if (canFire) {

                    // Try inflicting damage
                    StartCoroutine(LaserDamage(script));
                }
            }
        }
    }

    public virtual IEnumerator LaserDamage(EnemyHealth script) {

        for (int i = 0; i < numberOfBurstShots; i++) {

            switch (doesFireDamage) {
                case true:
                    script.TakeFireDamage(damage, script.transform.position - transform.position, knockback);
                    break;
                case false:
                    script.TakeDamage(damage, script.transform.position - transform.position, knockback);
                    break;
            }

            yield return new WaitForSeconds(timeBetweenBulletBurst);
        }
        
        parent.bursting = false;
        canFire = false;
    }

    public virtual void UpdateVisual(bool value, Vector3 end = default) {

        // Play firing sound
        triggerSound = true;

        switch (value) {
            case true:

                // Sets start value
                Vector3 start = lineRenderer.gameObject.transform.position;

                // Sets starting position
                lineRenderer.SetPosition(0, start);

                // Max out segment counts at the start
                segmentCount = maxSegmentCount;

                // Get the appropriate equal length for every segment
                Vector3 maxPosition = Vector3.Lerp(start, end, 1f / segmentCount);

                // If the segment lengths' are too small—
                while ((maxPosition - start).magnitude < minSegmentLength && segmentCount > 2) {

                    // Reduce the number of segments
                    if (segmentCount - 1 > 2) {
                        segmentCount--;
                    } else {
                        segmentCount = 2;
                    }

                    // Recalculate segment lengths
                    maxPosition = Vector3.Lerp(start, end, 1f / segmentCount);
                }

                // Initializes list if empty
                if (positions == null || positions.Count <= 0 || positions.Count != segmentCount) {
                    positions = new (segmentCount);

                    for (int i = 0; i < segmentCount; i++) {
                        positions.Add(Vector3.zero);
                    }
                }

                // Sets the number of line segments in the laser
                lineRenderer.positionCount = segmentCount;

                // Sets positions in between start and end
                for (int i = 1; i < segmentCount; i++) {

                    Vector3 distance = end - start;

                    Vector3 position = Vector3.Lerp(start, end, (float)i / segmentCount);

                    // Sets the initial position when laser is first fired
                    if (positions[i] == Vector3.zero) {
                        positions[i] = position;
                        lineRenderer.SetPosition(i, position);
                    }

                    // Randomize at a certain time interval
                    if (canRandomize) {

                        Vector3 perpendicular = Vector3.Cross(distance, new(0, 0, 1)).normalized * (Random.Range(-1f, 1f) * variationScale);

                        positions[i] = position + perpendicular;
                    }
                }

                // Reset randomization
                if (canRandomize) {
                    canRandomize = false;
                }

                // Sets the end position
                lineRenderer.SetPosition(segmentCount - 1, end);

                if (lineRenderer.enabled == false) {
                    lineRenderer.enabled = true;
                }

                break;
            case false:

                if (lineRenderer.enabled == true) {
                    lineRenderer.enabled = false;
                }

                positions?.Clear();
                break;
        }
    }

    public virtual void MoveSegments() {

        // For every segment in the line renderer—
        for (int i = 1; i < lineRenderer.positionCount - 1; i++) {

            // Get current position
            Vector3 currentPos = lineRenderer.GetPosition(i);

            float elasticTarget = (positions[i] - currentPos).magnitude;

            if (elasticTarget > elasticMax) {
                lineRenderer.SetPosition(i, positions[i]);
            } else {
                lineRenderer.SetPosition(i, Vector3.MoveTowards(currentPos, positions[i], segmentMoveSpeed * Time.deltaTime));
            }
        }
    }
}