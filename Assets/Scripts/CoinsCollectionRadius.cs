using UnityEngine;

public class CoinsCollectionRadius : MonoBehaviour
{

    [SerializeField] protected Coin parent;

    private PlayerController cont;

    private void Start() {

        if (GameStateManager.GetState() != GAMESTATE.GAMEOVER && parent != null) {
            if (parent.player.gameObject.TryGetComponent<PlayerController>(out var controller)) {
                cont = controller;
            }
        }
    }

    private void Update() {

        if (cont != null) {

            if (parent.transform.position.x <= GameStateManager.ToWorldPoint(cont.coinsUI.transform.position, parent.map.mainCam).x + 0.3f
            && parent.transform.position.x >= GameStateManager.ToWorldPoint(cont.coinsUI.transform.position, parent.map.mainCam).x - 0.3f
            && parent.transform.position.y <= GameStateManager.ToWorldPoint(cont.coinsUI.transform.position, parent.map.mainCam).y + 0.3f
            && parent.transform.position.y >= GameStateManager.ToWorldPoint(cont.coinsUI.transform.position, parent.map.mainCam).y - 0.3f) {

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
}
