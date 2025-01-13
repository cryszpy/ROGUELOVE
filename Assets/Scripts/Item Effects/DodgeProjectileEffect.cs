using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ItemEffect/DodgeProjectileEffect")]
public class DodgeProjectileEffect : ItemEffect 
{
    public float dodgeChance;

    public override void OnPickup()
    {
        PlayerController.EOnDodged += Use; // Subscribes item effect function to event OnDodged
        PlayerController.DodgeChance += dodgeChance;
    }

    public override void Use() {
        Debug.Log("DODGED!!");
        return;
    }
}