using System.Collections;
using UnityEngine;

public class AreaTransitionManager : MonoBehaviour
{
    public GameObject areaTransition;

    [SerializeField] private Animator areaTransitionAnimator;

    [SerializeField] private Animator areaTransitionTextAnimator;

    public void StartAreaTransition() {
        GameStateManager.transitionPauseLock = false;

        // If entering a new area (level is 1)—
        if (GameStateManager.GetLevel() == 1 && !GameStateManager.tutorialEnabled) {

            // Check to make sure animation objects aren't unassigned
            if (areaTransitionAnimator != null && areaTransition != null && areaTransitionTextAnimator != null) {

                // Activate transition object
                areaTransition.SetActive(true);

                // Start the transition animation
                StartCoroutine(TransitionToArea());
            }
        }
    }

    private IEnumerator TransitionToArea() {
        Debug.Log("PLAYING AREA TRANSITION");

        areaTransitionAnimator.SetTrigger("Open");
        areaTransitionTextAnimator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(2.5f);

        areaTransitionAnimator.SetTrigger("Close");
        areaTransitionTextAnimator.SetTrigger("FadeOut");
    }
}
