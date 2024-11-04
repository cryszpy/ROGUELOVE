using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/IgnoredCollisionsList")]
public class IgnoredCollisionsList : ScriptableObject
{
    public List<int> ignoredCollisions;
}
