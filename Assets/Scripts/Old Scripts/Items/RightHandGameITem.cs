using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandGameItem : GameItem
{
    public int defense;
    public RightHandGameItem(LootData lootData,int d) : base(lootData.spritePath, lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.RightHandSlot)
    {
        defense = d;
    }
}
