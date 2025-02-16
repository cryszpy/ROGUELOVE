using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ItemEffect/MoveSpeedEffect")]
public class MoveSpeedEffect : ItemEffect 
{
    [Tooltip("Set to 0 if no base speed modifier is in effect.")]
    public float moveSpeedAdditive;

    [Tooltip("Set to 1 if no % speed multiplier is in effect.")]
    public float moveSpeedMultiplicative;

    public override void OnPickup()
    {
        PlayerController.MoveSpeedMultiplier *= moveSpeedMultiplicative;
        PlayerController.ChangeMoveSpeed(moveSpeedAdditive);
    }
}