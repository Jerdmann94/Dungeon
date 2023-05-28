using System.Collections.Generic;
using Matchplay.Server;
using Newtonsoft.Json;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.JsonUtility;

public class InventoryManager : NetworkBehaviour

{
    // stuff
    public List<GameItem> lootInInventory;
    public GameObject inventoryParent;
    public PlayerController pc;

    public StaticReference attackOC;
    public StaticReference inventoryOC;

    public ListContainer listContainer;


    //Events
    public GameItemEvent resetUIEvent;
    [SerializeField] private GameUIRemoveEvent uiRemoveEvent;

    [SerializeField] private GameUISpawnEvent uiSpawnEvent;
    //UI element

    public GameObject uiItem;
    public Dictionary<OnDropType, GameItem> equipment;

    public override void OnNetworkSpawn()
    {
        equipment = new Dictionary<OnDropType, GameItem>
        {
            { OnDropType.HeadSlot, null },
            { OnDropType.ChestSlot, null },
            { OnDropType.LegSlot, null },
            { OnDropType.BootSlot, null },
            { OnDropType.LeftHandSlot, null },
            { OnDropType.RightHandSlot, null }
        };
        inventoryOC.target = inventoryParent;
        lootInInventory = new List<GameItem>();


        foreach (Transform child in inventoryParent.transform) Destroy(child);
        if (IsServer)
            ServerSingleton.Instance.Manager.NetworkServer.OnServerPlayerSpawned += SetUpInitialInventoryAndEquipment;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            ServerSingleton.Instance.Manager.NetworkServer.OnServerPlayerDespawned -= SetUpInitialInventoryAndEquipment;
        base.OnNetworkDespawn();
    }


    //FOR SERVER USE ONLY

    public async void SetUpInitialInventoryAndEquipment(Matchplayer matchplayer)
    {
        var userData = matchplayer.userData;
        var client = NetworkManager.ConnectedClients[userData.networkId];
        var ivObj = client.OwnedObjects[0].GetComponent<PlayerController>();
        var iv = ivObj.inventoryManager.GetComponent<InventoryManager>();
        ivObj.statBlock = new PlayerStatBlock();
        Debug.Log(client.ClientId);
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { client.ClientId }
            }
        };
        // get items from economy manager

        //Debug.Log(userData.userGamePreferences.equipment);
        var equip = JsonConvert.DeserializeObject<Dictionary<OnDropType, GameItem>>(userData.userGamePreferences
            .equipment);


        foreach (var keyValuePair in equip) iv.equipment[keyValuePair.Key] = keyValuePair.Value;
        var inv = JsonConvert.DeserializeObject<List<GameItem>>(userData.userGamePreferences.inventoryItems);

        foreach (var gameItem in inv) iv.lootInInventory.Add(gameItem);
        Debug.Log("Updating stats from init");


        //SENDING INFORMATION TO CLIENT


        foreach (var pair in iv.equipment)
        {
            if (pair.Value == null) continue;
            listContainer.itemsInServer.Add(pair.Value);
            MakeUIElementHereClientRpc(ToJson(pair.Value), pair.Key, clientRpcParams);
        }

        foreach (var item in iv.lootInInventory)
        {
            listContainer.itemsInServer.Add(item);
            MakeUIElementHereClientRpc(ToJson(item), OnDropType.Inventory, clientRpcParams);
        }
    }


    public void MoveItem(OnDropType locationBeingDroppedOn, GameItem item, OnDropType preDragLocation,
        string chestID, NetworkClient networkClient)
    {
        if (!IsServer)
            return;

        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { networkClient.ClientId }
            }
        };

        // need to determine if the item is really in the predrag location 
        if (!CheckForItemInPreDrag(item, preDragLocation, chestID, networkClient))
        {
            Debug.Log("Item " + item + " is not in that location " + preDragLocation +
                      " The item in that spot is " + GetItemAtPreDragEquipmentLocation(preDragLocation));
            return;
        }

        var removeItem =
            false; //BOOL TO DETERMINE IF WE SUCCESSFULLY MOVED THE ITEM AND NEED TO REMOVE IT FROM ITS ORIGIN
        switch (locationBeingDroppedOn) // location Item is dropped on
        {
            case OnDropType.Treasure:
                var treasure = listContainer.FindTreasure(chestID);
                //CHECK IF TREASURE IS CLOSE ENOUGH TO PLAYER TO ACTUALLY LOOT THIS ITEM
                if (!PositionCheck(treasure.transform.position, networkClient.PlayerObject.transform.position))
                    //send client method to close chest, 
                    break;
                //NEED TO ALSO CHECK THIS ON MOVEMENT TO SEE IF THE PLAYER HAS GONE TOO FAR AWAY
                // send client method to open chest
                treasure.AddItemToChest(item);
                removeItem = true;
                break;
            case OnDropType.Inventory:
                var playerController = networkClient.PlayerObject.GetComponent<PlayerController>();
                playerController.inventoryManager.lootInInventory.Add(item);
                //lootInInventory.Add(item);
                removeItem = true;
                break;
            case OnDropType.AttackPanel:
                if (item.onDropType != OnDropType.AttackPanel) break;
                var attackPanelManager = attackOC.GetComponent<AttackPanelManager>();
                attackPanelManager.runesInAttackPanel.Add(item);
                removeItem = true;
                break;
            case OnDropType.HeadSlot:
                removeItem = SlotChecking(OnDropType.HeadSlot, item, networkClient);
                break;
            case OnDropType.ChestSlot:
                removeItem = SlotChecking(OnDropType.ChestSlot, item, networkClient);
                break;
            case OnDropType.LegSlot:
                removeItem = SlotChecking(OnDropType.LegSlot, item, networkClient);
                break;
            case OnDropType.BootSlot:
                removeItem = SlotChecking(OnDropType.BootSlot, item, networkClient);
                break;
            case OnDropType.LeftHandSlot:
                removeItem = SlotChecking(OnDropType.LeftHandSlot, item, networkClient);
                break;
            case OnDropType.RightHandSlot:
                removeItem = SlotChecking(OnDropType.RightHandSlot, item, networkClient);
                break;
            case OnDropType.Nulling:
                removeItem = false;

                break;
        }


        if (!removeItem) return; // doing the stuff that happens if item is placed correctly
        //Debug.Log(item);
        RemoveItemFromPredragLocation(item, preDragLocation, chestID, networkClient);
        FixedString512Bytes v = ToJson(item);
        MakeUIElementHereClientRpc(v, locationBeingDroppedOn, clientRpcParams);
        pc.statBlock.UpdateStats(equipment);
        SendStatsToClientRpc(JsonConvert.SerializeObject(pc.statBlock), clientRpcParams);
    }

    private GameItem GetItemAtPreDragEquipmentLocation(OnDropType preDragLocation)
    {
        GameItem i = null;
        switch (preDragLocation)
        {
            case OnDropType.HeadSlot:
                i = equipment[OnDropType.HeadSlot];
                break;
            case OnDropType.ChestSlot:
                i = equipment[OnDropType.ChestSlot];
                break;
            case OnDropType.LegSlot:
                i = equipment[OnDropType.LegSlot];
                break;
            case OnDropType.BootSlot:
                i = equipment[OnDropType.BootSlot];
                break;
            case OnDropType.LeftHandSlot:
                i = equipment[OnDropType.LeftHandSlot];
                break;
            case OnDropType.RightHandSlot:
                i = equipment[OnDropType.RightHandSlot];
                break;
        }

        return i;
    }

    private bool CheckForItemInPreDrag(GameItem item, OnDropType preDragLocation, string chestID, NetworkClient client)
    {
        var playerController = client.PlayerObject.GetComponent<PlayerController>();
        var im = playerController.inventoryManager;
        switch (preDragLocation)
        {
            case OnDropType.Treasure:
                var treasure = listContainer.FindTreasure(chestID);
                return treasure.FindItemInChest(item) != null;
                break;
            case OnDropType.Inventory:

                return im.lootInInventory.Find(gameItem => gameItem.id == item.id) !=
                       null;
                break;
            case OnDropType.AttackPanel:

                break;
            case OnDropType.HeadSlot:
                return im.equipment[OnDropType.HeadSlot].id == item.id;
                break;
            case OnDropType.ChestSlot:
                return im.equipment[OnDropType.ChestSlot].id == item.id;
                break;
            case OnDropType.LegSlot:
                return im.equipment[OnDropType.LegSlot].id == item.id;
                break;
            case OnDropType.BootSlot:
                return im.equipment[OnDropType.BootSlot].id == item.id;
                break;
            case OnDropType.LeftHandSlot:
                return im.equipment[OnDropType.LeftHandSlot].id == item.id;
                break;
            case OnDropType.RightHandSlot:
                Debug.Log(im.equipment[OnDropType.RightHandSlot].id);
                Debug.Log(item.id);
                Debug.Log(im.equipment[OnDropType.RightHandSlot].id == item.id);
                return im.equipment[OnDropType.RightHandSlot].id == item.id;
                break;
            case OnDropType.Nulling:


                break;
        }

        return false;
    }

    private bool SlotChecking(OnDropType type, GameItem item, NetworkClient networkClient)
    {
        foreach (var VARIABLE in equipment.Keys)
        {
            //Debug.Log(VARIABLE);
        }

        //Debug.Log(type + " type and droptype " + item.onDropType);
        if (item.onDropType != type) return false; //CAN THIS ITEM BE A SLOT TYPE ITEM
        if (equipment[type] != null) // IS THERE ANOTHER ITEM THERE
            //move old item to inventory before moving forward

            MoveItem(OnDropType.Inventory, equipment[type], OnDropType.Nulling, null, networkClient);
        equipment[type] = item; //SET SLOT TO THIS ITEM
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { networkClient.ClientId }
            }
        };
        var json = ToJson(item); //SEND ITEM AS JSON TO CLIENT
        SetClientEquipmentClientRpc(json, type, clientRpcParams); // tell client to put item in ui

        return true;
    }

    // SERVER USE
    private bool PositionCheck(Vector3 transformPosition, Vector3 position)
    {
        return IsServer && PositionCheckUtility.PosCheck(transformPosition, position, 1.1f);
    }


    // SERVER USE
    private void RemoveItemFromPredragLocation(GameItem item, OnDropType preDragLocation, string chestID,
        NetworkClient client)
    {
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { client.ClientId }
            }
        };
        //Debug.Log(item + " predrag location " +
        //  preDragLocation +
        //  " chest ID " + chestID);
        var playerController = client.PlayerObject.GetComponent<PlayerController>();
        GameItem foundItem = null;
        switch (preDragLocation)
        {
            case OnDropType.Treasure:
                if (chestID == null) Debug.Log("Somehow we lost the chest ID on server side");
                var treasure = listContainer.FindTreasure(chestID);
                if (treasure == null) Debug.Log(chestID + " ChestID + treasure " + treasure);

                foundItem = treasure.FindItemInChest(item);
                if (foundItem == null)
                {
                    Debug.Log(
                        "Couldn't find Item in that chest, something has gone wrong, we have lost an Item somewhere");
                }
                else
                {
                    // Debug.Log("removing item from treasure");
                    treasure.RemoveItemFromChest(foundItem);
                    RemoveUIElementHereClientRpc(foundItem.id, OnDropType.Treasure, clientRpcParams);
                }

                break;
            case OnDropType.Inventory:
                var im = playerController.inventoryManager;
                foundItem = im.lootInInventory.Find(gameItem => gameItem == item);
                if (foundItem == null)
                {
                    Debug.Log(
                        "Couldn't find Item in Inventory, something has gone wrong, we have lost an Item somewhere");
                }
                else
                {
                    im.lootInInventory.Remove(foundItem);
                    RemoveUIElementHereClientRpc(foundItem.id, OnDropType.Inventory, clientRpcParams);
                }

                break;
            case OnDropType.AttackPanel:
                var attackPanelManager = attackOC.GetComponent<AttackPanelManager>();
                foundItem = attackPanelManager.runesInAttackPanel.Find(gameItem => gameItem == item);
                attackPanelManager.runesInAttackPanel.Remove(foundItem);
                break;
            case OnDropType.HeadSlot:
                equipment[OnDropType.HeadSlot] = null;
                RemoveUIElementHereClientRpc(item.id, OnDropType.HeadSlot, clientRpcParams);

                break;
            case OnDropType.ChestSlot:
                equipment[OnDropType.ChestSlot] = null;
                RemoveUIElementHereClientRpc(item.id, OnDropType.ChestSlot, clientRpcParams);

                break;
            case OnDropType.LegSlot:
                equipment[OnDropType.LegSlot] = null;
                RemoveUIElementHereClientRpc(item.id, OnDropType.LegSlot, clientRpcParams);

                break;
            case OnDropType.BootSlot:
                equipment[OnDropType.BootSlot] = null;
                RemoveUIElementHereClientRpc(item.id, OnDropType.BootSlot, clientRpcParams);

                break;
            case OnDropType.LeftHandSlot:
                equipment[OnDropType.LeftHandSlot] = null;
                RemoveUIElementHereClientRpc(item.id, OnDropType.LeftHandSlot, clientRpcParams);

                break;
            case OnDropType.RightHandSlot:
                equipment[OnDropType.RightHandSlot] = null;
                Debug.Log(item);
                RemoveUIElementHereClientRpc(item.id, OnDropType.RightHandSlot, clientRpcParams);

                break;
            case OnDropType.Nulling:
                break;
        }
    }

    /*[ClientRpc]
    private void SendInventoryIdForEconClientRpc(FixedString4096Bytes fixedBytes62)
    {
        throw new NotImplementedException();
    }
    private void SendEquipmentIdForEconClientRpc(FixedString4096Bytes fixedBytes62)
    {
        var equipArray = fixedBytes62.ToString().Split(" ");
        var options = new GetInventoryOptions();
        options.InventoryItemIds = equipArray.ToList();
        var e = EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);
        foreach (var resultPlayersInventoryItem in e.Result.PlayersInventoryItems)
        {
            var idef = await resultPlayersInventoryItem.GetItemDefinitionAsync();
            var cData=  idef.CustomDataDeserializable.GetAs<ItemCustomData>();
            var item = lootChangerUtil.StringLookUp(cData);
            equipment[item.onDropType] = item;
        }
    }*/

    /// /////////////////////////////////CLIENT SIDE STUFF///////////////////
    [ClientRpc] //SHOULD MAKE THIS THE ONLY WAY UI ELEMENTS ARE SPAWNED, SO EVERYTHING CORRESPONDS TO SERVER
    private void MakeUIElementHereClientRpc(FixedString512Bytes fixedString, OnDropType location,
        ClientRpcParams clientRpcParams)
    {

        var gi = LootChangerUtil.JsonToItem(fixedString.ToString());
        
        var go = Instantiate(uiItem);

        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gi.amountInThisStack.ToString();

        go.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + gi.sprite);
        //DOING SPRITES SOME OTHER WAY, WITH STRINGS TO FIND THEM
        var dd = go.GetComponent<DragAndDropLoot>();
        dd.item = gi;
        uiSpawnEvent.TriggerEvent(go, location);
    }

    [ClientRpc] //SHOULD MAKE THIS THE ONLY WAY UI ELEMENTS ARE REMOVED, SO EVERYTHING CORRESPONDS TO SERVER
    private void RemoveUIElementHereClientRpc(FixedString64Bytes itemID, OnDropType preDragLocation,
        ClientRpcParams clientRpcParams)
    {
//        Debug.Log("emitting removal of item at " + preDragLocation);
        uiRemoveEvent.TriggerEvent(itemID.ToString(), preDragLocation);
    }


    // not sure what to do with this yet, probably some kind of reset and sort function 
    private void ResetUI()
    {
        resetUIEvent.TriggerEvent(lootInInventory);
    }

    [ClientRpc]
    private void SetClientEquipmentClientRpc(FixedString512Bytes fixedString, OnDropType location,
        ClientRpcParams clientRpcParams)
    {
        var item = FromJson<GameItem>(fixedString.ToString());
        equipment[location] = item;
        //Debug.Log(location + " is now " + item);
    }

    public void UpdateInventoryDelegate(NetworkListEvent<FixedString512Bytes> changeEvent)
    {
        if (!IsClient)
            return;

        var json = changeEvent.Value.ToString();
        var index = changeEvent.Index;
        switch (changeEvent.Type)
        {
            case NetworkListEvent<FixedString512Bytes>.EventType.Add:
                lootInInventory.Add(FromJson<GameItem>(json));
                Debug.Log("Adding item to player Inventory, item is " + json);
                break;
            case NetworkListEvent<FixedString512Bytes>.EventType.Remove:
                Debug.Log("Using Remove json = " + json);
                var g = FromJson<GameItem>(json);
                Debug.Log("This is the item we are removing " + g);

                lootInInventory.Remove(lootInInventory.Find(item => item.id == g.id));
                break;
            case NetworkListEvent<FixedString512Bytes>.EventType.RemoveAt:
                Debug.Log("Removing item at index of " + index + " json = " + json);
                lootInInventory.RemoveAt(index);
                break;
            case NetworkListEvent<FixedString512Bytes>.EventType.Insert:
                break;
        }
    }

    [ClientRpc]
    public void SendStatsToClientRpc(string serializeObject, ClientRpcParams clientRpcParams)
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        pc.statBlock ??= new PlayerStatBlock();
        pc.statBlock.RecieveStatsFromServer(serializeObject);
        //DISPLAY STATS IN STAT PANEL
        Debug.Log("Should be updating player stats here");
        var statPanel = GameObject.FindWithTag("StatPanel");
        statPanel.GetComponent<StatPanelManager>().UpdateStatPanelText(pc.statBlock);
    }
}