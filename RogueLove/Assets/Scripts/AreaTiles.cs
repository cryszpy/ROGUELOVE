using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaTiles : MonoBehaviour
{
    [Header("TILEMAP OBJECTS")]

    public Tile floor;

    public Sprite ground;

    public RuleTile obstacles;

    public Tile decor;

    public Tile empty;

    public Sprite wallUp;

    public Sprite wallDown;

    public Sprite wallLeft;

    public Sprite wallRight;

    public GameObject doorwayObject;

    public GameObject[] breakables;
}
