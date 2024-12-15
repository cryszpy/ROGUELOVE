using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeRoomRadius : MonoBehaviour
{

    [SerializeField] private int room;
    
    [SerializeField] private HomeLookAt lookAt;

    public List<HomeRoomDividerRadius> dividers;
    
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {

            foreach (var divider in dividers) {
                if (divider.triggered) {
                    switch (room) {
                        case 1:
                            lookAt.room1 = true;
                            break;
                        case 2:
                            lookAt.room2 = true;
                            break;
                        case 3:
                            lookAt.room3 = true;
                            break;
                        case 4:
                            lookAt.room4 = true;
                            break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {

            foreach (var divider in dividers) {
                if (divider.triggered) {
                    switch (room) {
                        case 1:
                            lookAt.room1 = false;
                            break;
                        case 2:
                            lookAt.room2 = false;
                            break;
                        case 3:
                            lookAt.room3 = false;
                            break;
                        case 4:
                            lookAt.room4 = false;
                            break;
                    }
                }
            }
        }
    }
}
