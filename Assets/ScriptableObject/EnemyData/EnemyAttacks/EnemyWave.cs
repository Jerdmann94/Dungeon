using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "EnemyData/Attacks/EnemyWave")]
public class EnemyWave : EnemyAttack
{
    public WaveType waveType;
    public GameObject waveAttackObject;

    public Vector2Int[] wave1NS =
    {
        new(-1, 0),
        new(1, 0)
    };

    public Vector2Int[] wave1EW =
    {
        new(0, -1),
        new(0, 1)
    };
   
    public override void DoAttack(GameObject target, Transform ourPosition)
    {
       
        var acceptableLocations = new List<Vector3>();
        var currentPos = ourPosition.position;

        Vector3 dir = (Vector3Int.RoundToInt(target.transform.position) - Vector3Int.RoundToInt(currentPos));
//        Debug.Log(dir);
        dir /= dir.magnitude;
        dir = CardinalDirectionUtility.GetCardinalDirection(dir);
        Debug.Log(dir +" normalized");
        //THIS SWITCH IS FOR BUILDING THE ACCEPTABLE TILES IN EACH SHAPE
        switch (waveType)
        {
            case WaveType.Line:
                
                for (int i = 0; i < 5; i++)
                {
                    var hit = Physics2D.Raycast(currentPos, new Vector2(dir.x,dir.y),1, wallLayer);
                    Debug.DrawRay(currentPos, dir, Color.magenta, 1, false);
                    if (!hit)
                    {
                        currentPos += dir;
                        acceptableLocations.Add(currentPos);
                    }
                    else break;
                }
                break;
            case WaveType.Cone:
                for (int i = 0; i < 2;i++)
                {
                    var hit = Physics2D.Raycast(currentPos, new Vector2(dir.x,dir.y),.5f, wallLayer);
                    Debug.DrawRay(currentPos, dir, Color.cyan, 1, false);
                    if (!hit)
                    { 
                        currentPos += dir;
                        acceptableLocations.Add(currentPos);
                        //DOING THE SIDEWAYS EXPANSIONS OF THE WAVE
                        if (i != 1) continue;
                        //IF THIS IS 0, THEN WE ARE FACING NS
                        if (dir.x ==0)
                        {
                            foreach (var vector2Int in wave1NS)
                            {
//                                Debug.Log(vector2Int);
                                
                                var loc = new Vector2(dir.x + vector2Int.x, dir.y + vector2Int.y);
//                                Debug.Log(loc);
                                var test1 = Physics2D.Raycast(
                                    currentPos,vector2Int ,.5f, wallLayer);
                                Debug.DrawRay(currentPos, new Vector3(vector2Int.x,vector2Int.y,0), Color.cyan, 1.5f, false);
                                if (test1) continue;
                                var n = new Vector3(currentPos.x + vector2Int.x, currentPos.y + vector2Int.y, 0);
                                acceptableLocations.Add(n);
                            }
                        }
                        else
                        {
                            foreach (var vector2Int in wave1EW)
                            {
                                var loc = new Vector2(dir.x + vector2Int.x, dir.y + vector2Int.y);
                                var test1 = Physics2D.Raycast(
                                    currentPos,vector2Int,.5f, wallLayer);
                                Debug.DrawRay(currentPos, new Vector3(vector2Int.x,vector2Int.y,0), Color.cyan, 1.5f, false);
                                if (test1) continue;
                                var n = new Vector3(currentPos.x + vector2Int.x, currentPos.y + vector2Int.y, 0);
                                acceptableLocations.Add(n);
                            }
                            
                        }
                    }
                    else break;
                }
                break;
            case WaveType.Tomb:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (var location in acceptableLocations)
        {
            var loc = Vector3Int.RoundToInt(location);
            var o = Instantiate(waveAttackObject,loc,quaternion.identity);
            o.GetComponent<AttackScript>().Init(Random.Range(lowAttack, highAttack));
        }
    }
    
    

    
}

public enum WaveType
{
    Line,
    Cone,
    Tomb
}
