using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestGameItem : GameItem
{
    public int defense;
    public ChestGameItem(LootData lootData,int d) : base(lootData.spritePath, lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.ChestSlot)
    {
        defense = d;
    }
}
