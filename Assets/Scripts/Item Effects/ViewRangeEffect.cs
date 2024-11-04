using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ItemEffect/ViewRangeEffect")]
public class ViewRangeEffect : ItemEffect 
{
    public float viewRangeModifier;

    public override void OnPickup()
    {
        if (PlayerController.ViewRangeMultiplier == 1){
            PlayerController.ViewRangeMultiplier = viewRangeModifier;
        } else {
            PlayerController.ViewRangeMultiplier *= viewRangeModifier;
        }
    }
}