using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class AmuletGameItem : GameItem
{
    [JsonConstructor]
    public AmuletGameItem()
    {
    }
    public AmuletGameItem(LootData lootData) :base(lootData.spritePath,1,
    lootData.name,
    OnDropType.AmuletSlot, lootData.value){
        
    }

    
    
    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();
        t.name = name;
        t.rarity = rarity;
        t.dropType = onDropType;
        t.modBlocks = modBlocks;
        return t;
    }
}
