using Newtonsoft.Json;

public class LeftHandGameItem : GameItem
{
    public DamageType damageType;
    public int defense;

    [JsonConstructor]
    public LeftHandGameItem()
    {
    }

    public LeftHandGameItem(LootData lootData, int d, DamageType damageType) : base(lootData.spritePath,
        lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.LeftHandSlot)
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