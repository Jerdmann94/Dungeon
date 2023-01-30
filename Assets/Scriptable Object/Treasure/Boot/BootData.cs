using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Boots/Boot")]
public class BootData : LootData
{
    public int defense;
    public override object MakeGameContainer()
    {
        return new BootGameItem(this, defense);
    }
}
