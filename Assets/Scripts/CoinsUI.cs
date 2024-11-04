using UnityEngine;
using TMPro;

public class CoinsUI : MonoBehaviour
{

    [SerializeField] private TMP_Text coinsText;

    public void SetCoins (float coins) {

        if (coinsText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = coins.ToString();
        }

    }
}
