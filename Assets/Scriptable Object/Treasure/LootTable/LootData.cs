using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public abstract class LootData : ScriptableObject
{
    [SerializeField] private int chanceIn100;
    
    public int stackAmount;

    public int maxSpawnAmount;

    public string id;
    public string spritePath;
    public OnDropType useableLocation;

    public bool RollOnTable()
    {
        return (Random.Range(1, 101) < chanceIn100);
    }
    /*public virtual object MakeGameContainer()
    {
        Debug.Log("Using null game Container, this should not happen");
        return null;
    }*/

    public abstract GameItem MakeGameContainer();

}