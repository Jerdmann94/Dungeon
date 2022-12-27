using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TreasureScript : MonoBehaviour
{
    public LootTable lootTable;

    public List<GameItem> lootInThisChest;

    
    
    bool active = false;
    void Start()
    {
        
        lootInThisChest = new List<GameItem>();
        FillChestServerRPC(lootTable);
    }

    [ServerRpc]
    private void FillChestServerRPC(LootTable table)
    {
        foreach (var loot in table.loot)
        {
            //IF CHANGE TO ROLL WASNT LOW ENOUGH, CONTINUE TO NEXT ITERATION
            if (!loot.RollOnTable())
                continue;
            //SPAWN ACTUAL GAME CONTAINER, HAVE TO GET AWAY FROM SCRIPTABLE OBJECT HERE
            lootInThisChest.Add((GameItem)loot.MakeGameContainer());
            Debug.Log(lootInThisChest);
        }
    }
    [ServerRpc]
    public void AddItemToChestServerRpc(GameItem item)
    {
        lootInThisChest.Add(item);
    }
    [ServerRpc]
    public void RemoveItemFromChestServerRpc(GameItem item)
    {
        lootInThisChest.Remove(item);
    }
    
    
}
