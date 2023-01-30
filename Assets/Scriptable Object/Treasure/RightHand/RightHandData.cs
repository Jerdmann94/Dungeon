using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "coin",menuName = "Loot/Hands/Right/RightHandItem")]
public class RightHandData : LootData
{
    public int defense;
    public override object MakeGameContainer()
    {
        return new RightHandGameItem(this, defense);
    }
}
