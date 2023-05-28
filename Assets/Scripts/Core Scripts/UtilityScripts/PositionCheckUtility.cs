using UnityEngine;

public class PositionCheckUtility
{
    public static bool PosCheck(Vector3 start, Vector3 end, float dist)
    {
        var x = start.x - end.x;
        var y = start.y - end.y;
        return Mathf.Abs(x) < dist && Mathf.Abs(y) < dist;
    }
}