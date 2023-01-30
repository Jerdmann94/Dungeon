using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandGameItem : GameItem
{
    public int defense;
    public LeftHandGameItem(LootData lootData,int d) : base(lootData.spritePath, lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.LeftHandSlot)
    {
        defense = d;
    }
}
