using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "coin",menuName = "Loot/Coins/Copper")]
public class Coin : LootData
{
  

    public override GameItem MakeGameContainer(int maxSpawnAmount)
    {
        return new CoinStack(this,Random.Range(1,maxSpawnAmount));
    }
}
