using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core_Scripts.UtilityScripts;
using MyBox;
using Newtonsoft.Json.Linq;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHudView : MonoBehaviour
{
    public GameObject inventoryItemPrefab;
    public Transform stashOCTransform;
    public Transform sellingOCTransform;
    public LootChangerUtil lootChangerUtil;
    public LobbyInventoryManager inventoryManager;

    //equipment stuff
    public Transform headTransform;
    public Transform chestTransform;
    public Transform legTransform;
    public Transform bootTransform;
    public Transform leftTransform;
    public Transform rightTransform;
    public Transform ringTransform;
    public Transform amuletTransform;
    

    public async void FullRefresh(List<PlayersInventoryItem> playersInventoryItems)
    {
        // Check that scene has not been unloaded while processing async wait to prevent throw.
        if (inventoryItemPrefab == null || stashOCTransform == null) return;

        RemoveAllUI();
        inventoryManager.lootInSelling.Clear();
        inventoryManager.lootInStash.Clear();
        inventoryManager.equipment = EquipmentUtil.InitEquip();
        if (playersInventoryItems is null) return;
        
        foreach (var item in playersInventoryItems)
        {
            MakeItemUI(item);
        }

        Debug.Log("Inventory items retrieved and updated. Total inventory item count: " +
                  $"{playersInventoryItems.Count}");
    }

    public async void MakeItemUI(PlayersInventoryItem item)
    {
        InstanceData idata = null;
            
        Debug.Log(item.InstanceData.GetAsString());
        if (item.InstanceData.GetAsString()=="")
        {
                
        }
        else
        {
            JObject instanceText = JObject.Parse(item.InstanceData.GetAsString());
            InstanceData results = instanceText["InstanceData"].ToObject<InstanceData>();
            Debug.Log(results.ToString());
            //var idata = item.InstanceData.GetAs<InstanceData>();
            idata = results;
        }
//        Debug.Log(item.PlayersInventoryItemId);
        var idef = await item.GetItemDefinitionAsync();
//            Debug.Log(idef.Name);
        Debug.Log(idef.CustomDataDeserializable.GetAsString());
        var cData =  idef.CustomDataDeserializable.GetAs<ItemCustomData>();
        var newInventoryItemGameObject = Instantiate(inventoryItemPrefab, stashOCTransform);
        var dd = newInventoryItemGameObject.GetComponent<DragAndDropStore>();
        dd.data = cData;
        dd.econInventoryId = item.PlayersInventoryItemId;
        dd.preDragLocation = OnDropType.Inventory;
        Debug.Log(item.PlayersInventoryItemId + " " + item.InventoryItemId);
        var i = await lootChangerUtil.StringLookUp(cData, idata, item.PlayersInventoryItemId);
        dd.SetUpItem(i);
        dd.SetIconSprite(SpriteUtil.GetSprite(cData.sprite));
        i.sprite = cData.sprite;
        inventoryManager.lootInStash.Add(i);


    }

    //THIS REMOVES THE UI FROM THE STASH
    void RemoveAllUI()
    {
        while (stashOCTransform.childCount > 0)
        {
            DestroyImmediate(stashOCTransform.GetChild(0).gameObject);
        }
        while (sellingOCTransform.childCount > 0)
        {
            
            DestroyImmediate(sellingOCTransform.GetChild(0).gameObject);
        }

        var e = inventoryManager.equipment;

        if (e[OnDropType.HeadSlot] != null)
        {
            //e[OnDropType.HeadSlot] = null;
            while (headTransform.childCount > 0)
            {
                DestroyImmediate(headTransform.GetChild(0).gameObject);
            }
            
        }
        if (e[OnDropType.ChestSlot] != null)
        {
           // e[OnDropType.ChestSlot] = null;
            while (chestTransform.childCount > 0)
            {
                DestroyImmediate(chestTransform.GetChild(0).gameObject);
            }
            
        }
        if (e[OnDropType.LegSlot] != null)
        {
            //e[OnDropType.LegSlot] = null;
            while (legTransform.childCount > 0)
            {
                DestroyImmediate(legTransform.GetChild(0).gameObject);
            }
            
        }
        if (e[OnDropType.BootSlot] != null)
        {
           // e[OnDropType.BootSlot] = null;
            while (bootTransform.childCount > 0)
            {
                DestroyImmediate(bootTransform.GetChild(0).gameObject);
            }
            
        }
        if (e[OnDropType.LeftHandSlot] != null)
        {
           // e[OnDropType.LeftHandSlot] = null;
            while (leftTransform.childCount > 0)
            {
                DestroyImmediate(leftTransform.GetChild(0).gameObject);
            }
            
        }
        if (e[OnDropType.RightHandSlot] != null)
        {
           // e[OnDropType.RightHandSlot] = null;
            while (rightTransform.childCount > 0)
            {
                DestroyImmediate(rightTransform.GetChild(0).gameObject);
            }
            
        }
        if (e[OnDropType.AmuletSlot] != null)
        {
           // e[OnDropType.Amulet] = null;
            while (amuletTransform.childCount > 0)
            {
                DestroyImmediate(amuletTransform.GetChild(0).gameObject);
            }
            
        }
        if (e[OnDropType.RingSlot] != null)
        {
           // e[OnDropType.Ring] = null;
            while (ringTransform.childCount > 0)
            {
                DestroyImmediate(ringTransform.GetChild(0).gameObject);
            }

            
        }
        

    }

    public void RefreshItemsInInventory()
    {
        var selling = inventoryManager.lootInSelling;
        var stash = inventoryManager.lootInStash;
        RemoveAllUI();
       
        Debug.Log("items in stash " + stash.Count);
        foreach (var gameItem in stash)
        {
            SpawnItemUI(stashOCTransform, OnDropType.Stash,gameItem);
        }
        Debug.Log("items in selling " + selling.Count);
        foreach (var gameItem in selling)
        {
            SpawnItemUI(sellingOCTransform, OnDropType.Selling,gameItem);
        }
        
        //SPAWNING UI IN EACH EQUIP SLOT IF THERE IS A GAME ITEM THERE
        var e = inventoryManager.equipment;
        if (e[OnDropType.HeadSlot] != null)
        {
            SpawnItemUI(headTransform, OnDropType.HeadSlot,e[OnDropType.HeadSlot]);
        }
        if (e[OnDropType.ChestSlot] != null)
        {
            SpawnItemUI(chestTransform, OnDropType.ChestSlot,e[OnDropType.ChestSlot]);
        }
        if (e[OnDropType.LegSlot] != null)
        {
            SpawnItemUI(legTransform, OnDropType.LegSlot,e[OnDropType.LegSlot]);
        }
        if (e[OnDropType.BootSlot] != null)
        {
            SpawnItemUI(bootTransform, OnDropType.BootSlot,e[OnDropType.BootSlot]);
        }
        if (e[OnDropType.LeftHandSlot] != null)
        {
            SpawnItemUI(leftTransform, OnDropType.LeftHandSlot,e[OnDropType.LeftHandSlot]);
        }
        if (e[OnDropType.RightHandSlot] != null)
        {
            SpawnItemUI(rightTransform, OnDropType.RightHandSlot,e[OnDropType.RightHandSlot]);
        }
        if (e[OnDropType.AmuletSlot] != null)
        {
            SpawnItemUI(amuletTransform, OnDropType.AmuletSlot,e[OnDropType.AmuletSlot]);
        }
        if (e[OnDropType.RingSlot] != null)
        {
            SpawnItemUI(rightTransform, OnDropType.RingSlot,e[OnDropType.RingSlot]);
        }
        
        
    }

    public void SpawnItemUI(Transform t, OnDropType onDropType,GameItem gameItem)
    {
        Debug.Log(transform.gameObject.name);
        var newInventoryItemGameObject = Instantiate(inventoryItemPrefab, t);
        var dd = newInventoryItemGameObject.GetComponent<DragAndDropStore>();
        dd.preDragLocation = onDropType;
        dd.SetUpItem(gameItem);
        dd.SetIconSprite(SpriteUtil.GetSprite(gameItem.sprite));
    }
}


