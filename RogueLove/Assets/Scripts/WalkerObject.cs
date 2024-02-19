using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerObject
{
    // current position of the walker
    public Vector2 position;

    // direction of walker that it wants to go
    public Vector2 direction;

    // Gets compared for each method the walker follows
    public float chanceToChange;

    // Creates walker and fills parameters
    public WalkerObject(Vector2 pos, Vector2 dir, float chance) {
        position = pos;
        direction = dir;
        chanceToChange = chance;
    }
}
