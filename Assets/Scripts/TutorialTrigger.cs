using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    
    [SerializeField] private TutorialManager tutorialManager;

    private void OnEnable() {
        GameStateManager.EOnTutorialTrigger += Triggered;
    }

    private void OnDisable() {
        GameStateManager.EOnTutorialTrigger -= Triggered;
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.CompareTag("Player")) {
            GameStateManager.EOnTutorialTrigger?.Invoke();
        }
    }

    private void Triggered() {

        // Continues tutorial
        tutorialManager.ContinueTutorial();

        // Disables this trigger object
        gameObject.SetActive(false);
    }
}
