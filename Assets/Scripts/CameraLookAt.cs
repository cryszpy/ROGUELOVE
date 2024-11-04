using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private float viewRangeTracker;

    private void Start() {
        if (!playerTransform) {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            Debug.Log("Player transform is null! Reassigned.");
        }
        if (!mainCamera) {
            mainCamera = Camera.main;
            Debug.Log("Main camera is null! Reassigned.");
        }
    }
 
    private void Update()
    {
        if (GameStateManager.GetState() != GAMESTATE.MENU) {
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            float viewRange = Mathf.Clamp(PlayerController.ViewRangeBase * PlayerController.ViewRangeMultiplier, 2, 100);
            viewRangeTracker = viewRange;

            var cameraTargetPosition = (mousePosition + (viewRange - 1) * playerTransform.position) / viewRange;
            transform.position = cameraTargetPosition;
        }
    }
}
