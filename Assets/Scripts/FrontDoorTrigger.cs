using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDoorTrigger : MonoBehaviour
{
    private bool inRadius = false;

    [HideInInspector] public bool skipTutorial = false;

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

            // Play tutorial
            if (!HomeManager.TutorialDone && !skipTutorial) {

                GameStateManager.tutorialEnabled = true;

                GameStateManager.SetSave(false);

                TransitionManager.StartLeaf(1);

                Debug.Log("Started tutorial");
            } 
            // Skip tutorial
            else {
                
                GameStateManager.tutorialEnabled = false;

                GameStateManager.SetSave(false);

                TransitionManager.StartLeaf(1 + sceneInfo.sceneOffset);

                Debug.Log("Entered Front Door");
            }
        }
    }
}
