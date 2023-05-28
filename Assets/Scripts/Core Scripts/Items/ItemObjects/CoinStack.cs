using Newtonsoft.Json;
using UnityEngine;

public class CoinStack : GameItem
{
    [JsonConstructor]
    public CoinStack()
    {
    }

    public CoinStack(LootData lootData) : base(lootData.spritePath, lootData.maxSpawnAmount,
        Random.Range(1, lootData.maxSpawnAmount),
        lootData.name,
        OnDropType.Inventory)
    {
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();
        t.rarity = rarity;
        t.dropType = onDropType;
        return t;
    }
}