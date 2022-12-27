using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DropOnMe : MonoBehaviour, IDropHandler
{
    
    public StaticReference inventoryOc;
    
    public TreasureScript treasureScript;

    public OnDropType onDropType;


    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragAndDropLoot dragAndDropLoot = dropped.GetComponent<DragAndDropLoot>();
        dragAndDropLoot.SetParentTransform(this.gameObject.transform);

        switch (onDropType)
        {
            case OnDropType.Treasure :
                treasureScript.AddItemToChestServerRpc(dragAndDropLoot.item);
                inventoryOc.target.transform.parent.parent.parent.GetComponent<InventoryManager>().RemoveItemFromInventoryServerRpc(dragAndDropLoot.item);
                return;
            case OnDropType.Inventory:
                treasureScript.RemoveItemFromChestServerRpc(dragAndDropLoot.item);
                inventoryOc.target.transform.parent.parent.parent.GetComponent<InventoryManager>().AddItemToInventoryServerRpc(dragAndDropLoot.item);
                return;
        }
        
    }
}

public enum OnDropType {
    Treasure,
    Inventory,
    AttackPanel
}

