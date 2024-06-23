using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthBar : MonoBehaviour
{
    private enum HEARTSTATUS {
        EMPTY, FULL
    }

    [SerializeField]
    private Slider slider;

    [SerializeField] private Vector2 startPos;
    [SerializeField] private GameObject parentTo;

    [SerializeField] private float xOffset;

    [SerializeField] private List<int> healthList = new();
    [SerializeField] private List<int> updateHealthList = new();
    [SerializeField] private List<GameObject> heartList = new();

    [SerializeField] private GameObject heartObject;

    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [SerializeField]
    private Image fill;

    [SerializeField]
    private TMP_Text currentHealthText;

    [SerializeField]
    private TMP_Text maxHealthText;

    private void Awake() {
        if (parentTo == null) {
            parentTo = GameObject.FindWithTag("PlayerHealth");
            Debug.Log("HealthBar parentTo object was null! Reassigned.");
        }
    }

    public void SetMaxHealth (float health) {

        // First set of max health
        if (healthList.Count == 0){

            // For each point of health
            for (int i = 0; i < health; i++) {

                // Appends health to the list
                updateHealthList.Add((int)HEARTSTATUS.EMPTY);
                healthList.Add((int)HEARTSTATUS.EMPTY);

                // Spawn heart at each offset interval from spawn position
                GameObject spawnedHeart = Instantiate(heartObject, new Vector2(startPos.x + (xOffset * i), startPos.y), Quaternion.identity, parentTo.transform);

                // Adds heart GameObject to initialized list
                heartList.Add(spawnedHeart);

            }
        } 
        // GAINING MAX HEALTH
        else {
            for (int i = healthList.Count; i < health; i++) {

                // Adds health to the list
                updateHealthList.Add((int)HEARTSTATUS.EMPTY);
                healthList.Add((int)HEARTSTATUS.EMPTY);

                // Spawn heart at each offset interval from spawn position
                GameObject spawnedHeart = Instantiate(heartObject, new Vector2(startPos.x + (xOffset * i), startPos.y), Quaternion.identity, parentTo.transform);

                // Adds heart GameObject to initialized list
                heartList.Add(spawnedHeart);
            }
        }

        //slider.maxValue = health;
        //slider.value = health;
        if (maxHealthText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = health.ToString();
        }

    }

    public void SetHealth (int health) {

        //slider.value = health;

        // Clear the update list
        for (int i = 0; i < updateHealthList.Count; i++) {
            updateHealthList[i] = (int)HEARTSTATUS.EMPTY;
        }

        // Update the update list
        for (int i = 0; i < health; i++) {
            updateHealthList[i] = (int)HEARTSTATUS.FULL;
        }

        // Iterate over new health list
        for (int i = 0; i < updateHealthList.Count; i++) {
            if (updateHealthList[i] != healthList[i]) {
                SwapHearts(i, updateHealthList[i]);
            }
        }

        if (currentHealthText.TryGetComponent<TextMeshProUGUI>(out var letters)) {
            letters.text = health.ToString();
        }

    }

    private void SwapHearts(int oldHeartIndex, int newHeart) {

        switch (newHeart) {

            // If heart slot is LOST
            case 0:

                // Update heart status to empty
                healthList[oldHeartIndex] = newHeart;

                // Update heart sprite to empty
                if (heartList[oldHeartIndex].TryGetComponent<Image>(out var spriteRenderer)) {
                    spriteRenderer.sprite = emptyHeart;
                }
                else {
                    Debug.LogError("Could not find SpriteRenderer Component of heart to update!");
                }
                break;

            // If heart slot is FULL
            case 1:

                // Update heart status to full
                healthList[oldHeartIndex] = newHeart;

                // Update heart sprite to empty
                if (heartList[oldHeartIndex].TryGetComponent<Image>(out var renderer)) {
                    renderer.sprite = fullHeart;
                }
                else {
                    Debug.LogError("Could not find SpriteRenderer Component of heart to update!");
                }
                break;
            default:
                Debug.LogWarning("SwapHearts ran default case and didn't do anything.");
                break;
        }
    }
}
