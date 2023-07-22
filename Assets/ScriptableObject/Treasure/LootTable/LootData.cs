using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public abstract class LootData : ScriptableObject
{
    public int value;
    public string id;
    public string spritePath;
    public OnDropType useableLocation;

    public bool RollOnTable(int chanceIn1000)
    {
        return (Random.Range(1, 1001) < chanceIn1000);
    }
    public abstract GameItem MakeGameContainer(int maxSpawnAmount);

}