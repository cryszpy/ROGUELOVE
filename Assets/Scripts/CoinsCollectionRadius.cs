using UnityEngine;

public class CoinsCollectionRadius : MonoBehaviour
{

    [SerializeField] protected Coin parent;

    private PlayerController cont;

    private void Start() {

        if (parent.player.gameObject.TryGetComponent<PlayerController>(out var controller)) {
            cont = controller;
        }

    }

    private void Update() {

        if (cont != null) {

            if (parent.transform.position.x <= ToWorldPoint(cont.coinsUI.transform.position).x + 0.3f
            && parent.transform.position.x >= ToWorldPoint(cont.coinsUI.transform.position).x - 0.3f
            && parent.transform.position.y <= ToWorldPoint(cont.coinsUI.transform.position).y + 0.3f
            && parent.transform.position.y >= ToWorldPoint(cont.coinsUI.transform.position).y - 0.3f) {

                parent.PostEnemyDeath();

                switch (parent.coinType) {
                    case CoinType.BRONZE:
                        PlayerController.AddCoins(1);
                        break;
                    case CoinType.SILVER:
                        PlayerController.AddCoins(5);
                        break;
                    case CoinType.GOLD:
                        PlayerController.AddCoins(20);
                        break;
                }

                cont.coinsUI.SetCoins(PlayerController.Coins);
            }
        }
    }

    private Vector2 ToWorldPoint(Vector3 input) {

        Vector2 inCamera;
        Vector2 pixelAmount;
        Vector2 worldPoint;

        inCamera.y = parent.map.mainCam.orthographicSize * 2;
        inCamera.x = inCamera.y * Screen.width / Screen.height;

        pixelAmount.x = Screen.width / inCamera.x;
        pixelAmount.y = Screen.height / inCamera.y;

        worldPoint.x = ((input.x / pixelAmount.x) - (inCamera.x / 2) + parent.map.mainCam.transform.position.x);
        worldPoint.y = ((input.y / pixelAmount.y) - (inCamera.y / 2) + parent.map.mainCam.transform.position.y);

        return worldPoint;
    }
}
