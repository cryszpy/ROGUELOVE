using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponInfo : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public Image weaponSprite;

    [SerializeField] private TMP_Text currentAmmoText;

    [SerializeField] private TMP_Text maxEnergyText;

    [SerializeField] private float maxAmmo;

    public void SetMaxAmmo (float ammo) {
        maxAmmo = ammo;
        slider.maxValue = maxAmmo;
    }

    public void SetAmmo (float ammo, Weapon weapon) {

        // Sets slider to appropriate setting
        slider.value = ammo;

        if (currentAmmoText.TryGetComponent<TextMeshProUGUI>(out var letters)) {

            // If the weapon has infinite ammo, simply show the infinity symbol
            if (weapon.infiniteAmmo){
                letters.text = "∞";
            } 
            // Otherwise, show percentages (Magic-based weapons) or ratios (Tech-based weapons)
            else {

                switch (weapon.type) {
                    case WeaponType.MAGIC:
                        int percentage = Mathf.RoundToInt((ammo / maxAmmo) * 100f);
                        letters.text = percentage.ToString() + "%";
                        break;
                    case WeaponType.TECHNOLOGY:
                        letters.text = ammo.ToString() + "/" + maxAmmo.ToString();
                        break;
                    case WeaponType.SPECIAL:
                        break;
                }
            }
        }
    }
}
