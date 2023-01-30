using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.JsonUtility;

public class TreasureScript : NetworkBehaviour
{

    private List<GameItem> lootInThisChest;

    public ListContainer listContainer;

    public NetworkList<FixedString512Bytes> chestJson;
    
    
    
    bool active = false;
    [SerializeField] public string id;

    private void Awake()
    {
        chestJson = new NetworkList<FixedString512Bytes>();
    }

    public override void OnNetworkSpawn()
    {
        
        chestJson.OnListChanged += ChestValueChanged;
        if (IsServer)
        {
            //chestJson.Add(new FixedString512Bytes());
            /*id = Guid.NewGuid().ToString();
            lootInThisChest = new List<GameItem>();
            FillChest(lootTable);
            listContainer.treasureScripts.Add(this);*/
        }

        GetItemsForInitServerRpc(); // GET CLIENT ITEMS IN CHEST FROM SERVER
        

        base.OnNetworkSpawn();
       
       
        
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetItemsForInitServerRpc(ServerRpcParams rpcParams = default)
    {
        
        var fixedString = new FixedString4096Bytes ();
        fixedString = ToJson(new JsonListWrapper<GameItem>(lootInThisChest));
       
        /*foreach (var item in lootInThisChest)
        {
            fixedString +=(JsonUtility.ToJson(item));
        }
*/
        Debug.Log("FIXED STRING " + fixedString);
        
        GiveClientItemsClientRpc(fixedString,id);
    }

    [ClientRpc]
    private void GiveClientItemsClientRpc( FixedString4096Bytes fixedString,string serverId)
    {
        lootInThisChest = LootChangerUtil.LootExchanging(fixedString);
        id = serverId;
        //Debug.Log(lootInThisChest.Count);
    }
    
    private void ChestValueChanged(NetworkListEvent<FixedString512Bytes> changeEvent)
    {
        if (IsServer)
            return;
        
       //Debug.Log("CHEST VALUE HAS CHANGED AND SHOULD UPDATE, INSIDE CHEST DELEGATE");
        FixedString4096Bytes changeJson = changeEvent.Value;
        
        LootExchangeSingle(changeJson.ToString(),changeEvent.Index,changeEvent.Type);
    }

    private void LootExchangeSingle(string json, int index,
        NetworkListEvent<FixedString512Bytes>.EventType changeEventType)
    {
        switch (changeEventType)
        {
            case NetworkListEvent<FixedString512Bytes>.EventType.Add:
                lootInThisChest.Add(FromJson<GameItem>(json));
                //Debug.Log("Adding item to chest, item is " + json);
                break;
            case NetworkListEvent<FixedString512Bytes>.EventType.Remove:
               // Debug.Log("Using Remove json = " +json);
                var g = FromJson<GameItem>(json);
                //Debug.Log("This is the item we are removing " + g);
                
               lootInThisChest.Remove(lootInThisChest.Find(item => item.id == g.id));
                break;
            case NetworkListEvent<FixedString512Bytes>.EventType.RemoveAt:
                //Debug.Log("Removing item at index of "+ index+ " json = " +json);
                lootInThisChest.RemoveAt(index);
                break;
            case NetworkListEvent<FixedString512Bytes>.EventType.Insert:
                break;
        }
    }

    /*private void LootExchanging(FixedString512Bytes  changeJson)
    {
        
        var val = FromJson<JsonListWrapper<GameItem>>(changeJson.ToString()).list;
        lootInThisChest = val;
    }*/


    public void FillChest(LootTable table)
    {
        id = Guid.NewGuid().ToString();
        lootInThisChest = new List<GameItem>();
        var ts = listContainer.DoesAChestAlreadyExistThere(transform.position);
        if (listContainer.DoesAChestAlreadyExistThere(transform.position) != null)
        {
            AddItemsToChestAtThisLocation(table, ts);
            GetComponent<NetworkObject>().Despawn();
           
        }
        else
        {
            listContainer.treasureScripts.Add(this);
            AddItemsToChestAtThisLocation(table,this);
        }
        
    }

    private void AddItemsToChestAtThisLocation(LootTable table, TreasureScript ts)
    {
        foreach (var loot in table.loot)
        {
            //IF CHANGE TO ROLL WASNT LOW ENOUGH, CONTINUE TO NEXT ITERATION
            if (!loot.RollOnTable())
                continue;
            //SPAWN ACTUAL GAME CONTAINER, HAVE TO GET AWAY FROM SCRIPTABLE OBJECT HERE
            var i = (GameItem)loot.MakeGameContainer();
            ts.lootInThisChest.Add(i);
            ts.listContainer.itemsInServer.Add(i);
            ts.chestJson.Add(ToJson(i));
        } 
    }


    public void AddItemToChest(GameItem item)
    {
        if (!IsServer)
            return;
        lootInThisChest.Add(item);
        chestJson.Add(ToJson(item));
    }

    

    public void RemoveItemFromChest(GameItem item) // SERVER ONLY METHOD
    {
        if (!IsServer)
            return;
        
        var i = lootInThisChest.FindIndex(gameItem => gameItem == item);
        
        lootInThisChest.Remove(item);
        Debug.Log("Removing "+ item+ " from chest at index "+i);
        chestJson.Remove(ToJson(item));
        var log2 = "";
        foreach (var VARIABLE in chestJson)
        {
            log2 += VARIABLE;
        }
        Debug.Log(log2);

    }

    public void UpdateItemInChest()
    {
        if (!IsServer)
            return;
    }

    public GameItem FindItemInChest(GameItem searchItem)
    {
        if (!IsServer)
            return null;
        GameItem g = null;
        g = lootInThisChest.Find(gameItem => gameItem == searchItem);
        return g;
    }

    public List<GameItem> GiveItemsForUI()
    {
        return IsServer ? null : lootInThisChest;
    }


}
