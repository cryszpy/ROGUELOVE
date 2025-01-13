using System.Collections;
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
                target = ToWorldPoint(controller.coinsUI.transform.position);
            }

            transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.fixedDeltaTime);
        }
    }

    private Vector2 ToWorldPoint(Vector3 input) {

        Vector2 inCamera;
        Vector2 pixelAmount;
        Vector2 worldPoint;

        inCamera.y = map.mainCam.orthographicSize * 2;
        inCamera.x = inCamera.y * Screen.width / Screen.height;

        pixelAmount.x = Screen.width / inCamera.x;
        pixelAmount.y = Screen.height / inCamera.y;

        worldPoint.x = ((input.x / pixelAmount.x) - (inCamera.x / 2) + map.mainCam.transform.position.x);
        worldPoint.y = ((input.y / pixelAmount.y) - (inCamera.y / 2) + map.mainCam.transform.position.y);

        return worldPoint;
    }

    private IEnumerator Emerge(Vector2 fuerza) {
        rb.AddForce(Time.fixedDeltaTime * wanderSpeed * fuerza, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        coinSpawn = false;
    }

    public override void PostEnemyDeath() {
        Destroy(gameObject);
    }
}
