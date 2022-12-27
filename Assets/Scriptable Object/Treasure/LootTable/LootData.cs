using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class LootData : ScriptableObject
{
    [SerializeField]private int chanceIn100;

    public Sprite sprite;
    
    public int stackAmount;

    public int maxSpawnAmount;
    public bool RollOnTable()
    {
        return (Random.Range(1, 101) < chanceIn100);
    }
    public virtual object MakeGameContainer()
    {
        Debug.Log("Using null game Container, this should not happen");
        return null;
    }

   
}
