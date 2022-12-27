using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Coin",menuName = "Coin")]
public class Coin : LootData
{
    public int value;

    public override object MakeGameContainer()
    {
        return new CoinStack(this);
    }
}
