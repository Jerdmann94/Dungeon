using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootGameItem : GameItem
{
    public int defense;
    public BootGameItem(LootData lootData,int d) : base(lootData.spritePath, lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.BootSlot)
    {
        defense = d;
    }
}
