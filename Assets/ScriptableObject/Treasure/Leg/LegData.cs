using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "coin",menuName = "Loot/Legs/Leg")]
public class LegData : LootData
{
    public int defense;
    public DamageType damageType;
    public override GameItem MakeGameContainer(int maxAmount)
    {
        return new LegGameItem(this,defense,damageType);
    }
}
