using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "EnemyData/Attacks/EnemyRangedAoe")]
public class EnemyRangedAoe : EnemyAttack
{
    public GameObject attackObject;
    public int boxSize;
    public override void DoAttack(GameObject target, Transform ourPosition)
    {
        var toSpawn = GetBoxPositions(target.transform.position, boxSize, .5f, wallLayer);
        Debug.Log(toSpawn.Count);
        foreach (var vec2 in toSpawn)
        {
           var o= Instantiate(attackObject, vec2, quaternion.identity);
            o.GetComponent<AttackScript>().Init(Random.Range(lowAttack, highAttack));

        }
    }
    
    //THIS IS THE CHECK FROM RANGED BASIC ATTACK
    public override bool CheckRangeForAttack(GameObject target,Vector3 ourPosition,LayerMask playerLayer,LayerMask wallLayer)
    {
        //Debug.Log(target.name + target.transform.position + " name and postition of target");
        var distance = target.transform.position - ourPosition;
        var direction = distance / distance.magnitude;
        //SINCE THIS IS RANGED, WE HAVE TO CHECK FOR WALLS BEFORE ATTACKING WITH NORMAL CHECK
        var hit = Physics2D.Linecast(ourPosition, target.transform.position, wallLayer);
     //   Debug.DrawRay(ourPosition, direction, Color.blue, 2, false);
//        Debug.Log("Did the ray cast for range hit a wall " + hit);
        return !hit && base.CheckRangeForAttack(target, ourPosition, playerLayer,wallLayer);
    }
    
    
    
    
   public static List<Vector2> GetBoxPositions(Vector2 center, int boxSize, float raycastDistance, LayerMask obstacleLayer, bool addExtras = false)
{
    List<Vector2> positions = new List<Vector2>();

    if (boxSize == 2)
    {
        // Include the center tile in the cross shape
        positions.Add(center);

        // Generate a cross shape around the center vector
        Vector2 up = center + Vector2.up;
        Vector2 down = center + Vector2.down;
        Vector2 left = center + Vector2.left;
        Vector2 right = center + Vector2.right;

        // Perform a raycast for each position in the cross shape
        if (Physics2D.Raycast(center, Vector2.up, raycastDistance, obstacleLayer).collider == null)
            positions.Add(up);

        if (Physics2D.Raycast(center, Vector2.down, raycastDistance, obstacleLayer).collider == null)
            positions.Add(down);

        if (Physics2D.Raycast(center, Vector2.left, raycastDistance, obstacleLayer).collider == null)
            positions.Add(left);

        if (Physics2D.Raycast(center, Vector2.right, raycastDistance, obstacleLayer).collider == null)
            positions.Add(right);
    }
    else
    {
        int halfSize = boxSize / 2;

        // Calculate the positions for the box shape
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                Vector2 position = center + new Vector2(x, y);

                // Perform a raycast for each position in the box
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, raycastDistance, obstacleLayer);
                if (hit.collider != null)
                {
                    // Exclude positions with walls
                    continue;
                }

                positions.Add(position);
            }
        }
    }

    // Add extra small shapes on the edges of the boxes if the addExtras flag is true
    if (addExtras)
    {
        AddExtraShapes(center, positions, boxSize, raycastDistance, obstacleLayer);
    }

    return positions;
}

    private static void ExcludeFarSideTiles(Vector2 center, Vector2 hitPosition, List<Vector2> positions, int boxSize)
    {
        // Calculate the direction from the center to the hit position
        Vector2 direction = hitPosition - center;

        // Calculate the far side position
        Vector2 farSidePosition = hitPosition + direction;

        // Check if the far side position is already in the positions list
        if (positions.Contains(farSidePosition))
        {
            return;
        }

        // Remove the far side position from the positions list, if present
        positions.Remove(farSidePosition);

        // Check if the far side position is outside the box, and adjust positions accordingly
        int halfSize = boxSize / 2;
        int offset = halfSize % 2 == 0 ? 0 : 1; // Offset for odd-sized boxes

        if (Mathf.Abs(farSidePosition.x - center.x) > halfSize - offset || Mathf.Abs(farSidePosition.y - center.y) > halfSize - offset)
        {
            for (int x = -halfSize + offset; x <= halfSize; x++)
            {
                for (int y = -halfSize + offset; y <= halfSize; y++)
                {
                    Vector2 position = center + new Vector2(x, y);
                    positions.Remove(position);
                }
            }
        }
    }
    
    private static void AddExtraShapes(Vector2 center, List<Vector2> positions, int boxSize, float raycastDistance, LayerMask obstacleLayer)
    {
        // Add a cross shape extending off the edges of the boxes
        for (int i = -boxSize / 2; i <= boxSize / 2; i++)
        {
            Vector2 horizontalPosition = center + new Vector2(i, 0f);
            Vector2 verticalPosition = center + new Vector2(0f, i);

            // Perform a raycast from the center to the horizontal position
            RaycastHit2D horizontalHit = Physics2D.Raycast(center, horizontalPosition - center, raycastDistance, obstacleLayer);
            if (horizontalHit.collider == null)
            {
                positions.Add(horizontalPosition);
            }

            // Perform a raycast from the center to the vertical position
            RaycastHit2D verticalHit = Physics2D.Raycast(center, verticalPosition - center, raycastDistance, obstacleLayer);
            if (verticalHit.collider == null)
            {
                positions.Add(verticalPosition);
            }
        }
    }
}
