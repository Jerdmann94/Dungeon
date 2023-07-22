using Newtonsoft.Json;

public class BootGameItem : GameItem
{
    public DamageType damageType;
    public int defense;

    [JsonConstructor]
    private BootGameItem()
    {
    }

    public BootGameItem(LootData lootData, int d, DamageType damageType) : base(lootData.spritePath,d
        , lootData.name, OnDropType.BootSlot, lootData.value)
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