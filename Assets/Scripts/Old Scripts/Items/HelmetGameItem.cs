using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetGameItem : GameItem
{
    public int defense;

    public HelmetGameItem(LootData lootData,int d) : base(lootData.spritePath, lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.HeadSlot)
    {
        defense = d;
    }
}
