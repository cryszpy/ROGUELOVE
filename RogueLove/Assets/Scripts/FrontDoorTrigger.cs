using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDoorTrigger : DoorTrigger
{

    public override void Update() {
        
        if (inRadius && Input.GetKeyDown(KeyCode.E)) {
            GameStateManager.SetSave(false);

            TransitionManager.StartLeaf(1 + 1);

            Debug.Log("Entered Front Door");
        }
    }
}
