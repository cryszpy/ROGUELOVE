using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{

    // SAVE FILE PATH
    private string pathHome;

    [SerializeField] private PlayerController playerCont;

    // Player max health
    private static int something;
    public static int Something { get => something; set => something = value; }

    public static void ChangeSomething(int num) {
        Something += num;
    }

    public static int GetSomething() {
        return Something;
    }

    void Awake()
    {
        GameStateManager.SetState(GAMESTATE.PLAYING);

        pathHome = Application.persistentDataPath + "/home.soni";

        playerCont.PlayerStart(true);

        SaveHome();

        TransitionManager.EndLeaf(true);
    }

    public void SaveHome () {
        SaveSystem.SaveHome(this, playerCont.saveIcon);
        Debug.Log("SAVE HOME CALLED");
    }

    public void LoadHome() {
        
        // Load save data
        HomeData data = SaveSystem.LoadHome();

        Something = data.something;

        Debug.Log("LOADED HOME");
    }

}
