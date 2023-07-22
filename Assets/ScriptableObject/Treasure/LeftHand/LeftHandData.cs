using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "coin",menuName = "Loot/Hands/Left/LeftHandItem")]
public class LeftHandData : LootData
{
    public int defense;
    public DamageType damageType;
    public override GameItem MakeGameContainer(int maxAmount)
    {
        return new LeftHandGameItem(this, defense,damageType);
    }
}
