using System;
using Newtonsoft.Json;

[Serializable]
public class LegGameItem : GameItem
{
    public DamageType damageType;
    public int defense;

    [JsonConstructor]
    public LegGameItem()
    {
    }

    public LegGameItem(LootData lootData, int d, DamageType damageType) : base(lootData.spritePath,
       1, lootData.name, OnDropType.LegSlot, lootData.value)
    {
        this.damageType = damageType;
        defense = d;
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();
        t.name = name;
        t.name = name;
        t.defense = defense;
        t.rarity = rarity;
        t.dropType = onDropType;
        t.damageType = damageType;
        t.modBlocks = modBlocks;
        return t;
    }
}