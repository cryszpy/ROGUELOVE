using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake() {
        GameStateManager.SetStage(0);
        GameStateManager.SetLevel(0);
        GameStateManager.SetState(GAMESTATE.MENU);
    }
}
