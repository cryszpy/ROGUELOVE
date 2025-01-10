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
            Vector3 mousePosition = ToWorldPoint(Input.mousePosition);

            float viewRange = Mathf.Clamp(PlayerController.ViewRangeBase * PlayerController.ViewRangeMultiplier, 2, 100);
            viewRangeTracker = viewRange;

            var cameraTargetPosition = (mousePosition + (viewRange - 1) * playerTransform.position) / viewRange;
            transform.position = cameraTargetPosition;
        }
    }

    private Vector2 ToWorldPoint(Vector3 input) {

        Vector2 inCamera;
        Vector2 pixelAmount;
        Vector2 worldPoint;

        inCamera.y = mainCamera.orthographicSize * 2;
        inCamera.x = inCamera.y * Screen.width / Screen.height;

        pixelAmount.x = Screen.width / inCamera.x;
        pixelAmount.y = Screen.height / inCamera.y;

        worldPoint.x = ((input.x / pixelAmount.x) - (inCamera.x / 2) + mainCamera.transform.position.x);
        worldPoint.y = ((input.y / pixelAmount.y) - (inCamera.y / 2) + mainCamera.transform.position.y);

        return worldPoint;
    }
}
