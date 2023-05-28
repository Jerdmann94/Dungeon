using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHudView : MonoBehaviour
{
    public GameObject inventoryItemPrefab;
    public Transform itemListParentTransform;
    public LootChangerUtil lootChangerUtil;


    public async void Refresh(List<PlayersInventoryItem> playersInventoryItems)
    {
        // Check that scene has not been unloaded while processing async wait to prevent throw.
        if (inventoryItemPrefab == null || itemListParentTransform == null) return;

        RemoveAll();
        
        if (playersInventoryItems is null) return;
        
        foreach (var item in playersInventoryItems)
        {
            InstanceData idata = null;
            //Debug.Log(item.InstanceData.GetAsString());
            if (item.InstanceData.GetAsString()=="")
            {
                
            }
            else
            {
                JObject instanceText = JObject.Parse(item.InstanceData.GetAsString());
                InstanceData results = instanceText["InstanceData"].ToObject<InstanceData>();
               // Debug.Log(results.ToString());
                //var idata = item.InstanceData.GetAs<InstanceData>();
                idata = results;
            }
            
            var idef = await item.GetItemDefinitionAsync();
            var cData =  idef.CustomDataDeserializable.GetAs<ItemCustomData>();
            var newInventoryItemGameObject = Instantiate(inventoryItemPrefab, itemListParentTransform);
            var dd = newInventoryItemGameObject.GetComponent<DragAndDropStore>();
            dd.data = cData;
            dd.econInventoryId = item.PlayersInventoryItemId;
            dd.preDragLocation = OnDropTypeStore.Inventory;
            //Debug.Log(item.PlayersInventoryItemId + " " + item.InventoryItemId);
            var i = await lootChangerUtil.StringLookUp(cData, idata, item.PlayersInventoryItemId);
            dd.SetUpItem(i);
            newInventoryItemGameObject.GetComponent<DragAndDropStore>().SetIconSprite(Resources.Load<Sprite>(cData.sprite));
           
        }

        Debug.Log("Inventory items retrieved and updated. Total inventory item count: " +
                  $"{playersInventoryItems.Count}");
    }

    void RemoveAll()
    {
        while (itemListParentTransform.childCount > 0)
        {
            DestroyImmediate(itemListParentTransform.GetChild(0).gameObject);
        }
    }
}


