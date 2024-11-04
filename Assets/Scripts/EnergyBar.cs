using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyBar : MonoBehaviour
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
    private TMP_Text currentEnergyText;

    [SerializeField]
    private TMP_Text maxEnergyText;

    public void SetMaxEnergy (float energy) {

        slider.maxValue = energy;
        //slider.value = health;
        if (maxEnergyText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = energy.ToString();
        }
        //fill.color = gradient.Evaluate(1f);

    }

    public void SetEnergy (float energy) {

        slider.value = energy;
        if (currentEnergyText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = energy.ToString();
        }

        //fill.color = gradient.Evaluate(slider.normalizedValue);

    }
}
