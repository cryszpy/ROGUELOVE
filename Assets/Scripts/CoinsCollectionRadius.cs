using System.Collections;
using System.Collections.Generic;
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

            if (parent.transform.position.x <= parent.map.mainCam.ScreenToWorldPoint(cont.coinsUI.transform.position).x + 0.3f
            && parent.transform.position.x >= parent.map.mainCam.ScreenToWorldPoint(cont.coinsUI.transform.position).x - 0.3f
            && parent.transform.position.y <= parent.map.mainCam.ScreenToWorldPoint(cont.coinsUI.transform.position).y + 0.3f
            && parent.transform.position.y >= parent.map.mainCam.ScreenToWorldPoint(cont.coinsUI.transform.position).y - 0.3f) {

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
