using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneList : MonoBehaviour
{
    public string[] scene;

    private int currentLvl = 0;
    
    // Loads selected scene
    public string loadScene(int n)
    {
        currentLvl = n;
        return scene[n];
    }
}
