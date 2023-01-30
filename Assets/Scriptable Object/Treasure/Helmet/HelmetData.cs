using System.Collections;
using System.Collections.Generic;
using UnityEngine;[CreateAssetMenu(menuName = "Loot/Helmets/Crown")]
public class HelmetData : LootData
{
    public int defense;
    
    
    public override object MakeGameContainer()
    {
        return new HelmetGameItem(this,defense);
    }
}
