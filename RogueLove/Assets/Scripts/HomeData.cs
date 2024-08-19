using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class HomeData
{

    public int something;

    public HomeData (HomeManager manager) {

        something = HomeManager.GetSomething();
        
    }
}
