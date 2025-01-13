using UnityEngine;

public class AreaTransition : MonoBehaviour
{

    [SerializeField] private AreaTransitionManager areaTransitionManager;

    public void HideAreaTransition() {
        
        if (!areaTransitionManager) {
            areaTransitionManager = GameObject.FindGameObjectWithTag("TransitionManager").GetComponentInChildren<AreaTransitionManager>();
        }

        if (areaTransitionManager.areaTransition != null) {
            areaTransitionManager.areaTransition.SetActive(false);
        }
    }
}
