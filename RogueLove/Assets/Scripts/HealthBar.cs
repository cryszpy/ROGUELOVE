using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    [SerializeField]
    private Slider slider;

    /*
    [SerializeField]
    private Gradient gradient;
    */

    [SerializeField]
    private Image fill;

    [SerializeField]
    private TMP_Text currentHealthText;

    [SerializeField]
    private TMP_Text maxHealthText;

    public void SetMaxHealth (float health) {

        slider.maxValue = health;
        //slider.value = health;
        if (maxHealthText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = health.ToString();
        }
        //fill.color = gradient.Evaluate(1f);

    }

    public void SetHealth (float health) {

        slider.value = health;
        if (currentHealthText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = health.ToString();
        }

        //fill.color = gradient.Evaluate(slider.normalizedValue);

    }
}
