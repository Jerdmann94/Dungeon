using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "coin",menuName = "Loot/Coins/Copper")]
public class Coin : LootData
{
    public int value;

    public override GameItem MakeGameContainer()
    {
        return new CoinStack(this);
    }
}
