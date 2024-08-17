using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeManager : MonoBehaviour
{

    // SAVE FILE PATH
    private string pathHome;

    [SerializeField] private PlayerController playerCont;

    void Awake()
    {
        GameStateManager.SetState(GAMESTATE.PLAYING);

        pathHome = Application.persistentDataPath + "/home.soni";

        playerCont.PlayerStart(true);
    }

}
