using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "chest",menuName = "Loot/Chest/ChestArmor")]
public class ChestArmorData : LootData
{
    public int defense;
    
    
    public override object MakeGameContainer()
    {
        return new ChestGameItem(this,defense);
    }
}
