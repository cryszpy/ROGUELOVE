using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTransition : MonoBehaviour
{

    [SerializeField] private AreaTransitionManager areaTransitionManager;

    public void HideAreaTransition() {
        if (areaTransitionManager.areaTransition != null) {
            areaTransitionManager.areaTransition.SetActive(false);
        }
    }
}
