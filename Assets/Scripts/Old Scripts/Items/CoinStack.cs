using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class CoinStack : GameItem
{
   
    public CoinStack(LootData lootData) : base(lootData.spritePath,lootData.maxSpawnAmount, 
        Random.Range(1, lootData.maxSpawnAmount),
        lootData.name,
        OnDropType.Inventory)
    {
        
    }
}
