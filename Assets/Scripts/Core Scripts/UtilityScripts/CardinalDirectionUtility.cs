using UnityEngine;

public static class CardinalDirectionUtility
{
    public static Vector2 GetCardinalDirection(Vector2 normalizedVector)
    {
        Vector2 cardinalDirection = Vector2.zero;

        // Determine the largest component of the normalized vector
        float maxComponent = Mathf.Max(Mathf.Abs(normalizedVector.x), Mathf.Abs(normalizedVector.y));

        // Assign the appropriate cardinal direction based on the largest component
        if (Mathf.Abs(normalizedVector.x) == maxComponent)
        {
            cardinalDirection = (normalizedVector.x > 0) ? Vector2.right : Vector2.left;
        }
        else if (Mathf.Abs(normalizedVector.y) == maxComponent)
        {
            cardinalDirection = (normalizedVector.y > 0) ? Vector2.up : Vector2.down;
        }

        return cardinalDirection;
    }
}