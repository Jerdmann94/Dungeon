using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matchplay.Client;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Economy;
using UnityEngine;

public class TeleporterScript : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (IsServer) return;
        //        Debug.Log(col);
        if (col.CompareTag("Player")) PlayerExtractionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerExtractionServerRpc(ServerRpcParams rpcParams = default)
    {
        //GET PC SOMEHOW
        var clientId = rpcParams.Receive.SenderClientId;
        var client = NetworkManager.ConnectedClients[clientId];
        var pc = client.OwnedObjects[0].GetComponent<PlayerController>();
        DealWithInventoryInExtraction(pc, clientId);
    }


    private async void DealWithInventoryInExtraction(PlayerController pc, ulong client)
    {
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { client }
            }
        };
        foreach (var gameItem in pc.inventoryManager.lootInInventory)
            AddClientItemToInventoryClientRpc(JsonConvert.SerializeObject(gameItem), clientRpcParams);
        //await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(gameItem.name, options);

        foreach (var kp in pc.inventoryManager.equipment)
        {
            if (kp.Value == null) continue;
            var gameItem = kp.Value;
            AddClientItemToInventoryClientRpc(JsonConvert.SerializeObject(gameItem), clientRpcParams);
        }

        await Task.Delay(1000);
        DisconnectClientAfterTeleportClientRpc(clientRpcParams);
    }

    [ClientRpc]
    public void AddClientItemToInventoryClientRpc(FixedString512Bytes item, ClientRpcParams clientRpcParams = default)
    {
        //THSI WILL ALL NEED TO BE CHANGED
        //I CANT REMEMBER WHY I WROTE IT THIS WAY
        var instanceData = new Dictionary<string, object>();

        var gameItem = LootChangerUtil.JsonToItem(item.ToString());
        Debug.Log(gameItem.name);
        Debug.Log(gameItem.GetType());
        if (gameItem.GetType() == typeof(GameItem))
        {
            DoSellingItem(gameItem);
            return;
        }
        var iData = new InstanceData();
        instanceData.Add("Rarity", gameItem.rarity);
        instanceData.Add("ItemId", gameItem.id);
        instanceData.Add("AmountInThisStack", gameItem.amountInThisStack);
        instanceData.Add("modBlocks", gameItem.modBlocks);
        instanceData = LootChangerUtil.AddInstanceSpecificData(gameItem, instanceData);
        iData.rarity = gameItem.rarity;
        iData.damageType = (DamageType)instanceData["damageType"];
        iData.itemId = gameItem.id;
        iData.modBlocks = gameItem.modBlocks;
        iData.amountInThisStack = gameItem.amountInThisStack;
        var i = new Dictionary<string, InstanceData>();
        i.Add("InstanceData", iData);
        var options = new AddInventoryItemOptions
        {
            PlayersInventoryItemId = gameItem.id,
            InstanceData = i
        };
        SendCalltoEconomy(gameItem.name.ToUpper(), options);
    }

    private async void DoSellingItem(GameItem gameItem)
    {
        Debug.Log(gameItem.name +" name and value "+ gameItem.value);
        if (gameItem.value < 1)
        {
            return;
        }
        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("COIN", gameItem.value*gameItem.amountInThisStack);
    }


    private async void SendCalltoEconomy(string name, AddInventoryItemOptions options)
    {
        var eName = name.Replace(' ', '_');
        eName = eName.Replace("'", "_");
        
        Debug.Log(eName + " name and options, last call befor fail " + options);
        try
        {
            var i = await EconomyService.Instance.PlayerInventory.AddInventoryItemAsync(eName, options);

        }
        catch (EconomyValidationException e)
        {
            Console.WriteLine(e.Details);
            
            throw;
        }
      
    }


    [ClientRpc]
    private void DisconnectClientAfterTeleportClientRpc(ClientRpcParams clientRpcParams = default)
    {
        ClientSingleton.Instance.Manager.Disconnect();
    }
}