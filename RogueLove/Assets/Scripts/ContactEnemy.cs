using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;
using NUnit.Framework.Constraints;
using System;

public class ContactEnemy : Enemy
{
    protected Vector3 tile;

    private int direc;

    private EnemyType type = EnemyType.CONTACT;

    public override void Chase() {
        force = speed * Time.deltaTime * direction.normalized;
        
        rb.AddForce(force);
    }

    public override void Wander() {
        Debug.Log("Wandering");
        canWander = false;
        waiting = false;

        StartCoroutine(Roam());
        return;
    }

    public IEnumerator Roam() {

        direc = UnityEngine.Random.Range(0, 8);
        moveTime = UnityEngine.Random.Range(1, 3);
        waitTime = UnityEngine.Random.Range(2, 5);

        switch (direc) {
            case 0:
                force = wanderSpeed * Time.deltaTime * Vector2.up;
                yield return null;
                break;
            case 1:
                force = wanderSpeed * Time.deltaTime * Vector2.down;
                yield return null;
                break;
            case 2:
                force = wanderSpeed * Time.deltaTime * Vector2.right;
                yield return null;
                break;
            case 3:
                force = wanderSpeed * Time.deltaTime * Vector2.left;
                yield return null;
                break;
            case 4:
                force = wanderSpeed * Time.deltaTime * Vector2.zero;
                yield return null;
                break;
            case 5:
                force = wanderSpeed * Time.deltaTime * Vector2.up;
                force += wanderSpeed * Time.deltaTime * Vector2.right;
                yield return null;
                break;
            case 6:
                force = wanderSpeed * Time.deltaTime * Vector2.up;
                force += wanderSpeed * Time.deltaTime * Vector2.left;
                yield return null;
                break;
            case 7:
                force = wanderSpeed * Time.deltaTime * Vector2.down;
                force += wanderSpeed * Time.deltaTime * Vector2.right;
                yield return null;
                break;
            case 8:
                force = wanderSpeed * Time.deltaTime * Vector2.down;
                force += wanderSpeed * Time.deltaTime * Vector2.left;
                yield return null;
                break;
            default:
                direc = UnityEngine.Random.Range(0, 4);
                yield return null;
                break;
        }
        Debug.Log("Set Direction");
        while (canWander == false && !waiting) {
            Debug.Log("IN THE LOOP");
            rb.AddForce(force);
            yield return null;
        }
        yield return null;
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