using System;

using System.Collections.Generic;
using Unity.Mathematics;

using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "EnemyData/Attacks/AoeAround")]
public class AoeAround : EnemyAttack
{
    public int aoeRange;
    public AOEType aoeType;
    public GameObject aoeEffectPrefab;

    public override void DoAttack(GameObject target, Transform ourPosition)
    {
        Debug.Log("inside aoe do attack");
        var outerEdges = new List<Vector2>();
        var position = ourPosition.position;
        for (int i = 0; i <= aoeRange; i++)
        {
            outerEdges.Add(new Vector2(position.x+aoeRange,position.y+i));
            outerEdges.Add(new Vector2(position.x-aoeRange,position.y-i));
            outerEdges.Add(new Vector2(position.x+i,position.y+aoeRange));
            outerEdges.Add(new Vector2(position.x-i,position.y-aoeRange));
        }

        var potentialTiles = new List<Vector2>();
        var correctPos = new List<Vector2>();
        foreach (var vec2 in outerEdges)
        {
           var t=  GetTilesBetweenPoints(ourPosition.position, vec2);
           foreach (var betweenTile in t)
           {
               potentialTiles.Add(betweenTile);
           }

           if (!IsWallOnTile(vec2))
           {
               correctPos.Add(vec2);
           }
        }

        foreach (var vec2 in potentialTiles)
        {
            if (vec2 == Vector2Int.RoundToInt(ourPosition.position))
            {
                continue;
            }
            if (!IsWallOnTile(vec2))
            {
                correctPos.Add(vec2);
            }
        }

        SpawnAoeDamageTiles(correctPos);
        
    }

    private void SpawnAoeDamageTiles(List<Vector2> correctPos)
    {
        var damage = Random.Range(lowAttack, highAttack);
        foreach (var vec2 in correctPos)
        {
//            Debug.Log("spawning aoe effect");
            var o = Instantiate(aoeEffectPrefab,vec2,quaternion.identity);
            o.GetComponent<AttackScript>().Init(damage);
            
        }
        
    }

    public bool IsWallOnTile(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 0f, wallLayer);
        return hit.collider != null;
    }

    

    public override bool CheckRangeForAttack(GameObject target, Vector3 ourPosition, LayerMask playerLayer, LayerMask wallLayer)
    {
        // CHECK IF LOCATION IS CLOSE ENOUGH TO THIS GAMEOBJECT
        var distance = Vector3.Distance(target.transform.position, ourPosition);
        return distance <= attackRange;
    }
    
    public static List<Vector2> GetTilesBetweenPoints(Vector2 start, Vector2 end)
    {
        List<Vector2> tiles = new List<Vector2>();

        float x1 = start.x;
        float y1 = start.y;
        float x2 = end.x;
        float y2 = end.y;

        float dx = Math.Abs(x2 - x1);
        float dy = Math.Abs(y2 - y1);

        int sx = (x1 < x2) ? 1 : -1;
        int sy = (y1 < y2) ? 1 : -1;

        float err = dx - dy;

        while (true)
        {
            tiles.Add(new Vector2(x1, y1));

            if (x1 == x2 && y1 == y2)
                break;

            float e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }

        return tiles;
    }
}

public enum AOEType
{
    Damage,
    Healing,
    Haste
}
