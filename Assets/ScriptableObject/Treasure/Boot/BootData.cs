using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Boots/Boot")]
public class BootData : LootData
{
    public int defense;
    public DamageType damageType;
    public override GameItem MakeGameContainer(int a)
    {
        return new BootGameItem(this, defense,damageType);
    }
}
