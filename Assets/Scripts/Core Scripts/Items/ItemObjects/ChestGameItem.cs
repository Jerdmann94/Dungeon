using Newtonsoft.Json;
using UnityEngine;


public class ChestGameItem : GameItem
{
    public DamageType damageType;
    public int defense;

    [JsonConstructor]
    public ChestGameItem()
    {
    }

    public ChestGameItem(LootData lootData, int d, DamageType damageType) : base(lootData.spritePath,1
        , lootData.name, OnDropType.ChestSlot,lootData.value)
    {
        this.damageType = damageType;
        defense = d;
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();
        t.name = name;
        t.defense = defense;
        t.rarity = rarity;
        t.dropType = onDropType;
        t.damageType = damageType;
        t.modBlocks = modBlocks;
        return t;
    }
}