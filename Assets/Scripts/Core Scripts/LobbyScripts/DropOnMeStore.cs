using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropOnMeStore : NetworkBehaviour, IDropHandler
{
    // public StaticReference inventoryOc;

    [SerializeField] private OnDropType myDropType;

    public LobbyInventoryManager inventoryManager;

    public InventoryHudView hudView;

    //private Dictionary<OnDropType, GameItem> equipment;
    // public ListContainer listContainer;


    public void OnDrop(PointerEventData eventData)
    {
//        Debug.Log("Drop detected");
        if (IsServer)
            return;

        var dropped = eventData.pointerDrag;
        var dd = dropped.GetComponent<DragAndDropStore>();
        
        //NORMAL DROP STUFF
        
        Debug.Log("what is my drop type and item drop type " + dd.item.allowablePosition + " " + myDropType);
        
        
        //IF THIS ITEM IS IN THE WRONG PLACE, RETUN EARLY
        if (dd.item.allowablePosition != myDropType &&
            myDropType is not (OnDropType.Inventory or OnDropType.Stash or OnDropType.Selling)) return;
        
        
        // IF WE ARE ARE AN EQUIPMENT, DELETE THE OLD UI
        if (myDropType is not (OnDropType.Inventory or OnDropType.Stash or OnDropType.Selling))
        {
            if (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            
            //SET DROPPED UI TO THIS PARENT
            Debug.Log("next is set parent");
            dd.SetParentTransform(transform, myDropType);
        }
        
        




        //ALWAYS REMOVE ITEM FROM INVENTORY AND EQUIPMENT TO BE SURE,
        //THEN RE-ADD IT TO ANYWHERE
        inventoryManager.RemoveItemFromInventoryAndEquipment(dd.item);

        //CHECK IF THERE IS AN ITEM AT THE LOCATION WE A DROPPING FOR EQUIPMENT
        //AND ADD ITEM TO THIS LOCATION
        //equipment = inventoryManager.equipment;

        switch (myDropType)
        {
            case OnDropType.Inventory:
                inventoryManager.lootInInventory.Add(dd.item);
                break;
            case OnDropType.Selling:
                inventoryManager.lootInSelling.Add(dd.item);
                break;
            case OnDropType.Stash:
                inventoryManager.lootInStash.Add(dd.item);
                break;
            default:
                CheckSlotForItem(dd.item,dd.item.allowablePosition, inventoryManager.equipment);
               // Debug.Log("next is set parent");
               // dd.SetParentTransform(transform, myDropType);
                break;
        }

        hudView.RefreshItemsInInventory();
        inventoryManager.lobbyStatBlock.statBlock ??= new PlayerStatBlock();
        var statPanel = GameObject.FindWithTag("StatPanel");
        inventoryManager.lobbyStatBlock.statBlock.UpdateStats(inventoryManager.equipment);
        statPanel.GetComponent<StatPanelManager>().UpdateStatPanelText(inventoryManager.lobbyStatBlock.statBlock);
    }


    private void CheckSlotForItem(GameItem item, OnDropType dropType, Dictionary<OnDropType,GameItem> equips)
    {
        var gameItem = equips[dropType];
        if (gameItem != null)
        {
            if (gameItem != item)
            {
                inventoryManager.lootInStash.Add(gameItem);
            }
        }
        equips[dropType] = item;
        Debug.Log("Item at location: " + item + " "  + dropType);
    }
}

//removing this because it over complicates things for now. might add it later ifi need it
/*public enum OnDropType
{
    Selling,
    Store,
    Stash,
    Inventory,
    HeadSlot,
    ChestSlot,
    LegSlot,
    BootSlot,
    LeftHandSlot,
    RightHandSlot
}*/