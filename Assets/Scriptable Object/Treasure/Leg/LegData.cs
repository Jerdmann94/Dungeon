using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "coin",menuName = "Loot/Legs/Leg")]
public class LegData : LootData
{
    public int defense;
    public override object MakeGameContainer()
    {
        return new LegGameItem(this,defense);
    }
}
