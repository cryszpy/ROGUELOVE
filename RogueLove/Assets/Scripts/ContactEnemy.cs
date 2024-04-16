using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;
using NUnit.Framework.Constraints;
using System;

public class ContactEnemy : Enemy
{
    //protected Vector3 tile;

    public override void SetEnemyType() {
        enemyType = EnemyType.CONTACT;
    }

    /*
    public override IEnumerator SetTarget() {
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(10, 20));

        
        int rand = map.GetRandomTile();
        tile = new(map.tileListX[rand], map.tileListY[rand]);
        bool ihatethis = (Mathf.Abs(map.tileListX[rand] - this.transform.position.x) <= followCollider.radius) && (Mathf.Abs(map.tileListY[rand] - this.transform.position.y) <= followCollider.radius);
        Debug.Log(ihatethis);
        if (ihatethis) {
            
            target = tile;
            Debug.Log("Set Tile");
        } 
    } */


}