using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropOnMeStore : NetworkBehaviour, IDropHandler
{
    // public StaticReference inventoryOc;

    [SerializeField] private OnDropTypeStore myDropType;

    public LobbyInventoryManager inventoryManager;

    // public ListContainer listContainer;


    public void OnDrop(PointerEventData eventData)
    {
//        Debug.Log("Drop detected");
        if (IsServer)
            return;

        var dropped = eventData.pointerDrag;
        var dd = dropped.GetComponent<DragAndDropStore>();
        if (dd.data.onDropType == myDropType ||
            myDropType is OnDropTypeStore.Inventory or OnDropTypeStore.Stash or OnDropTypeStore.Selling)
            dd.SetParentTransform(transform, myDropType);

        //ALWAYS REMOVE ITEM FROM INVENTORY AND EQUIPMENT TO BE SURE,
        //THEN RE-ADD IT TO ANYWHERE
        inventoryManager.RemoveItemFromInventoryAndEquipment(dd.item);
        switch (myDropType)
        {
            case OnDropTypeStore.Inventory:
                inventoryManager.lootInInventory.Add(dd.item);
                break;
            case OnDropTypeStore.Selling:
                break;
            case OnDropTypeStore.Stash:
                break;
            case OnDropTypeStore.HeadSlot:
                inventoryManager.equipment[OnDropType.HeadSlot] = dd.item;
                break;
            case OnDropTypeStore.ChestSlot:
                inventoryManager.equipment[OnDropType.ChestSlot] = dd.item;
                break;
            case OnDropTypeStore.LegSlot:
                inventoryManager.equipment[OnDropType.LegSlot] = dd.item;
                break;
            case OnDropTypeStore.LeftHandSlot:
                inventoryManager.equipment[OnDropType.LeftHandSlot] = dd.item;
                break;
            case OnDropTypeStore.RightHandSlot:
                inventoryManager.equipment[OnDropType.RightHandSlot] = dd.item;
                break;
            case OnDropTypeStore.BootSlot:
                inventoryManager.equipment[OnDropType.BootSlot] = dd.item;
                break;
        }


        inventoryManager.lobbyStatBlock.statBlock ??= new PlayerStatBlock();
        // Debug.Log("Should be updating player stats here");
        var statPanel = GameObject.FindWithTag("StatPanel");
//        Debug.Log(statPanel);
        inventoryManager.lobbyStatBlock.statBlock.UpdateStats(inventoryManager.equipment);
        statPanel.GetComponent<StatPanelManager>().UpdateStatPanelText(inventoryManager.lobbyStatBlock.statBlock);

        //inventoryManager.lobbyStatBlock.statBlock.UpdateStats(inventoryManager.equipment);
        //Debug.Log(dragAndDropLoot.item + "  and my drop type " + myDropType+" " + dragAndDropLoot.preDragLocation+" " + treasureScript.id);
        //MoveItemServerRpc(dragAndDropLoot.item.id,myDropType, dragAndDropLoot.preDragLocation,treasureScript.id);
        //dragAndDropLoot.SetParentTransform(this.gameObject.transform, myDropType);
    }
}

public enum OnDropTypeStore
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
}