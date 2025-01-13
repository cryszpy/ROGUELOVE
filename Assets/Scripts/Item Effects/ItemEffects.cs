using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public virtual void OnPickup() {
        return;
    }

    public virtual void Use() {
        return;
    }
    
}
