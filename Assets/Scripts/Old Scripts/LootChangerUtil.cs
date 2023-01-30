using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LootChangerUtil 
{
    
    public static List<GameItem> LootExchanging(FixedString4096Bytes  changeJson)
    {
        var val = JsonUtility.FromJson<JsonListWrapper<GameItem>>(changeJson.ToString()).list;
        return val;
    }
    
}
