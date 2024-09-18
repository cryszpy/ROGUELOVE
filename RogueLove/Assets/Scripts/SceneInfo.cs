using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/SceneInfo")]
public class SceneInfo : ScriptableObject
{
    [Header("SCENE INFO")]

    public List<int> bossSceneIndexes;

    public int sceneOffset;
}
