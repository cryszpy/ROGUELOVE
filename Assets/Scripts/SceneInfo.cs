using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/SceneInfo")]
public class SceneInfo : ScriptableObject
{
    [Header("SCENE INFO")]

    public List<int> bossSceneIndexes;

    public int sceneOffset;
}
