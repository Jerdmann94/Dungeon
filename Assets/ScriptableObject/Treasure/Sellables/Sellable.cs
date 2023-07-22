using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "coin",menuName = "Loot/Sellable")]
public class Sellable : LootData
{
   // public int value;
    public override GameItem MakeGameContainer(int maxSpawnAmount)
    {
        return new SellableGameItem(this, 1,value);
    }
}


