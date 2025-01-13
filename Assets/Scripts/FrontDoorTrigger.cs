using UnityEngine;

public class FrontDoorTrigger : MonoBehaviour
{
    private bool inRadius = false;

    [SerializeField] private Animator useAnimator;

    [HideInInspector] public bool skipTutorial = false;

    [SerializeField] private SceneInfo sceneInfo;

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.CompareTag("Player")) {

            // Show use indicator
            UseIndicator(true);

            inRadius = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {

        if (collider.CompareTag("Player")) {

            // Hide use indicator
            UseIndicator(false);

            inRadius = false;
        }
    }

    // Show/hide use indicator
    public virtual void UseIndicator(bool value) {

        if (useAnimator) {
            useAnimator.SetBool("Show", value);
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
