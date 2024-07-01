using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaTiles : MonoBehaviour
{
    [Header("TILEMAP OBJECTS")]

    public RuleTile floor;

    public Sprite ground;

    public RuleTile obstacles;

    public RuleTile walls;

    public Tile empty;

    public Tile borderUp;
    public Tile borderDown;

    public Sprite wallUp;

    public Sprite wallDown;

    public Sprite wallLeft;

    public Sprite wallRight;

    public GameObject doorwayObject;

    public GameObject[] breakables;
}
