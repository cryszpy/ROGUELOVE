using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeLookAt : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<GameObject> points;
    [Range(2, 100)] [SerializeField] private float cameraTargetDivider;

    public bool room1;

    public bool room2 = true;

    public bool room3;

    public bool room4;

    private void Start() {
        room2 = true;
    }
 
    private void Update()
    {
        if (GameStateManager.GetState() != GAMESTATE.MENU) {

            if (room1) {
                var cameraTargetPosition = (playerTransform.position + (cameraTargetDivider - 1) * points[0].transform.position) / cameraTargetDivider;
                transform.position = cameraTargetPosition; 
            } else if (room2) {
                var cameraTargetPosition = (playerTransform.position + (cameraTargetDivider - 1) * points[1].transform.position) / cameraTargetDivider;
                transform.position = cameraTargetPosition; 
            } else if (room3) {
                var cameraTargetPosition = (playerTransform.position + (cameraTargetDivider - 1) * points[2].transform.position) / cameraTargetDivider;
                transform.position = cameraTargetPosition; 
            } else if (room4) {
                var cameraTargetPosition = (playerTransform.position + (cameraTargetDivider - 1) * points[3].transform.position) / cameraTargetDivider;
                transform.position = cameraTargetPosition; 
            }     
        }
    }
}
