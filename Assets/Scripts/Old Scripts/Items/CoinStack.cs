using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class CoinStack : GameItem
{
   
    public CoinStack(LootData lootData)
    {
        amountInThisStack = Random.Range(1, lootData.maxSpawnAmount);
        sprite = lootData.sprite;
        maxStackAmount = lootData.stackAmount;
        name = lootData.name;
    }
}
