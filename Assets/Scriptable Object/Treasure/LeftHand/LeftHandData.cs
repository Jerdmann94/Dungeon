using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "coin",menuName = "Loot/Hands/Left/LeftHandItem")]
public class LeftHandData : LootData
{
    public int defense;
    public override object MakeGameContainer()
    {
        return new LeftHandGameItem(this, defense);
    }
}
