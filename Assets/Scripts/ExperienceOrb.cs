using System.Collections;
using UnityEngine;

public class ExperienceOrb : ContactEnemy
{
    Vector2 dir1;
    Vector2 dir2;

    Vector2 combined;

    public override void Start()
    {
        base.Start();
        expSpawn = true;
    }

    public override void DirectionFacing()
    {
        return;
    }

    public override void PlayerCheck()
    {
        if (expSpawn) {
            
            // Choose random direction
            int rand = UnityEngine.Random.Range(0, 7);

            switch (rand) {
                // UP ^
                case 0:
                    dir1 = Vector2.up;

                    force = Vector2.zero;
                    combined = dir1;
                    StartCoroutine(Emerge(combined));
                    break;
                // UP LEFT ^\
                case 1:
                    dir1 = Vector2.up;
                    dir2 = Vector2.left;

                    force = Vector2.zero;
                    combined = dir1 * dir2;
                    StartCoroutine(Emerge(combined));
                    break;
                // LEFT <--
                case 2:
                    dir1 = Vector2.left;

                    force = Vector2.zero;
                    combined = dir1;
                    StartCoroutine(Emerge(combined));
                    break;
                // LEFT DOWN v/
                case 3:
                    dir1 = Vector2.left;
                    dir2 = Vector2.down;

                    force = Vector2.zero;
                    combined = dir1 * dir2;
                    StartCoroutine(Emerge(combined));
                    break;
                // DOWN v
                case 4:
                    dir1 = Vector2.down;

                    force = Vector2.zero;
                    combined = dir1;
                    StartCoroutine(Emerge(combined));
                    break;
                // DOWN RIGHT \v
                case 5:
                    dir1 = Vector2.right;
                    dir2 = Vector2.down;

                    force = Vector2.zero;
                    combined = dir1 * dir2;
                    StartCoroutine(Emerge(combined));
                    break;
                // RIGHT v
                case 6:
                    dir1 = Vector2.right;

                    force = Vector2.zero;
                    combined = dir1;
                    StartCoroutine(Emerge(combined));
                    break;
                // RIGHT UP /^
                case 7:
                    dir1 = Vector2.right;
                    dir2 = Vector2.up;

                    force = Vector2.zero;
                    combined = dir1 * dir2;
                    StartCoroutine(Emerge(combined));
                    break;
            }
            
        }
        if (!expSpawn) {
            target = player.transform.position;
            //Chase();
            transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Emerge(Vector2 fuerza) {
        rb.AddForce(Time.deltaTime * wanderSpeed * fuerza, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        expSpawn = false;
    }

    public override void PostEnemyDeath() {
        Destroy(gameObject);
    }
}
