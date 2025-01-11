using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Animator animator;

    [SerializeField] private Image image;

    [SerializeField] private TMP_Text nameText;

    [SerializeField] private float holdTime;

    private void Start() {
        FindReferences();
    }

    private void FindReferences() {
        if (GameStateManager.GetStage() != 0 && GameStateManager.GetState() != GAMESTATE.MENU) {
            if (player == null) {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                Debug.Log("Player controller was null! Reassigned.");
            }
            if (animator == null) {
                animator = GameObject.FindGameObjectWithTag("PickupInfo").GetComponent<Animator>();
                Debug.Log("Animator component was null! Reassigned.");
            }
            if (image == null) {
                image = GameObject.FindGameObjectWithTag("PickupInfoSprite").GetComponent<Image>();
                Debug.Log("Pickup info sprite was null! Reassigned.");
            }
            if (nameText == null) {
                nameText = GameObject.FindGameObjectWithTag("PickupInfoName").GetComponent<TextMeshProUGUI>();
                Debug.Log("Pickup info name text was null! Reassigned.");
            }
            /* if (typeText == null) {
                typeText = GameObject.FindGameObjectWithTag("PickupInfoType").GetComponent<TextMeshProUGUI>();
                Debug.Log("Pickup info flavor text was null! Reassigned.");
            } */
        }
    }

    public void StartItemAnimation() {

        FindReferences();

        image.sprite = player.heldItems[^1].imageSprite;
        nameText.text = player.heldItems[^1].pickupName;
        /* if (player.heldItems[^1].TryGetComponent<ItemPickup>(out var item)) {
            typeText.text = "Item Pickup!";
        } else if (player.heldItems[^1].TryGetComponent<WeaponPickup>(out var weapon)) {
            typeText.text = "Weapon Pickup!";
        } */

        StartCoroutine(PlayAnimation());
    }

    public void StartWeaponAnimation(Weapon weapon) {

        FindReferences();

        image.sprite = weapon.spriteRenderer.sprite;
        nameText.text = weapon.name;
        /* if (player.heldItems[^1].TryGetComponent<ItemPickup>(out var item)) {
            typeText.text = "Item Pickup!";
        } else if (player.heldItems[^1].TryGetComponent<WeaponPickup>(out var weapon)) {
            typeText.text = "Weapon Pickup!";
        } */

        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation() {

        if (animator) {
            animator.SetBool("Open", true);
        }

            yield return new WaitForSeconds(holdTime);

        if (animator) {
            animator.SetBool("Open", false);
        }
    }
}
