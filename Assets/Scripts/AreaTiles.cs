using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/AreaTiles")]
public class AreaTiles : ScriptableObject
{
    [Header("TILEMAP OBJECTS")]

    public RuleTile floor;

    public Sprite ground;

    public RuleTile obstacles;

    public RuleTile walls;

    public Tile empty;

    public Tile grass;

    public Tile borderUp;

    public Tile borderDown;

    public Tile borderLeft;

    public GameObject doorwayObject;

    public GameObject chest;
    public GameObject bigChest;

    public GameObject[] breakables;
}
