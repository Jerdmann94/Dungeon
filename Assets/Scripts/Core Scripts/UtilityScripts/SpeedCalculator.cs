using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCalculator
{
    private const float A = 1.4426950408889634f;
    private const float B = -0.4426950408889634f;

    //solve for a and b where : t = a/log(x) +b, when t = 1, x = 10, when t=0 x = 50
    public static float GetSpeedTimer(float speedStat)
    {
        //Debug.Log(speedStat);
        //Debug.Log(A / Mathf.Log(speedStat,10) + B);
        return A / Mathf.Log(speedStat,10) + B;
    }
}
