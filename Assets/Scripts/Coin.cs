using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinType {
    BRONZE, SILVER, GOLD
}

public class Coin : ContactEnemy
{
    Vector2 dir1;

    Vector2 combined;

    public CoinType coinType;

    public override void Start()
    {
        base.Start();
        coinSpawn = true;
    }

    public override void DirectionFacing()
    {
        return;
    }

    public override void PlayerCheck()
    {
        if (coinSpawn) {
            
            dir1 = Vector2.up;

            force = Vector2.zero;
            combined = dir1;
            StartCoroutine(Emerge(combined));
            
        }
        if (!coinSpawn) {

            if (player.gameObject.TryGetComponent<PlayerController>(out var controller)) {
                target = map.mainCam.ScreenToWorldPoint(controller.coinsUI.transform.position);
            }

            transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Emerge(Vector2 fuerza) {
        rb.AddForce(Time.fixedDeltaTime * wanderSpeed * fuerza, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        coinSpawn = false;
    }

    public override void RemoveEnemy() {
        Destroy(gameObject);
    }
}
