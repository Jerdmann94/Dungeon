using Newtonsoft.Json;
using UnityEngine;

public class CoinStack : GameItem
{

   
    
    [JsonConstructor]
    public CoinStack()
    {
    }

    public CoinStack(LootData lootData, int range) : base(lootData.spritePath,
        range,
        lootData.name,
        OnDropType.Inventory,lootData.value)
    {
   
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();
        t.name = name;
        t.amountInThisStack = amountInThisStack;
        
        return t;
    }
}