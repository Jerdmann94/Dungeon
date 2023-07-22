using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFramerate : MonoBehaviour
{

    public int target = 120;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }

}
