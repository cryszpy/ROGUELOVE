using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDoorTrigger : MonoBehaviour
{
    private bool inRadius = false;

    [SerializeField] private SceneInfo sceneInfo;

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.CompareTag("Player")) {
            inRadius = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (collider.CompareTag("Player")) {
            inRadius = false;
        }
    }

    public void Update() {
        
        if (inRadius && Input.GetKeyDown(KeyCode.E)) {
            GameStateManager.SetSave(false);

            TransitionManager.StartLeaf(1 + sceneInfo.sceneOffset);

            Debug.Log("Entered Front Door");
        }
    }
}
