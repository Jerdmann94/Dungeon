using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegGameItem : GameItem
{
    public int defense;
    public LegGameItem(LootData lootData,int d) : base(lootData.spritePath, lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.LegSlot)
    {
        defense = d;
    }
}
