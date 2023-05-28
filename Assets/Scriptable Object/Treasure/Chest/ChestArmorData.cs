using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "chest",menuName = "Loot/Chest/ChestArmor")]
public class ChestArmorData : LootData
{
    public int defense;
    public DamageType damageType;
    
    
    public override GameItem MakeGameContainer()
    {
        return new ChestGameItem(this,defense,damageType);
    }
}
