using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropOnMe : NetworkBehaviour, IDropHandler
{
    public StaticReference inventoryOc;

    public TreasureScript treasureScript;

    [SerializeField] private OnDropType myDropType;


    public ListContainer listContainer;


    private void Awake()
    {
        listContainer.clientCollectionsThatNeedTreasureScript.Add(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("Drop detected");
        if (IsServer)
            return;

        //Debug.Log(myDropType);
        var dropped = eventData.pointerDrag;
        var dragAndDropLoot = dropped.GetComponent<DragAndDropLoot>();

        if (treasureScript == null)
        {
        //    Debug.Log(dragAndDropLoot.item + "  and my drop type " + myDropType+" PredragLocation is" + dragAndDropLoot.preDragLocation);
            MoveItemServerRpc(dragAndDropLoot.item.id, myDropType, dragAndDropLoot.preDragLocation, "");

        }
        else
        {
//            Debug.Log(dragAndDropLoot.item + "  and my drop type " + myDropType+" " + dragAndDropLoot.preDragLocation+" " + treasureScript.id);
            MoveItemServerRpc(dragAndDropLoot.item.id, myDropType, dragAndDropLoot.preDragLocation, treasureScript.id);

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveItemServerRpc(string itemID,
        OnDropType dropLocation,
        OnDropType preDragLocation,
        string chestId,
        ServerRpcParams serverRpcParams = default)
    {
       Debug.Log("Inside move item server rpc");
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        var i = listContainer.FindItem(itemID);
        Debug.Log("This should be found item " + i);
        var inventoryManager = inventoryOc.Target.GetComponent<ReferenceToInventoryManager>().inventoryManager;
        inventoryManager.MoveItem(dropLocation, i, preDragLocation, chestId, client);
    }

    public void OnUISpawn(GameObject go, OnDropType type)
    {
        // Debug.Log(this.myDropType + " object name" + gameObject.name);
        if (type != myDropType)
            return;
        go.GetComponent<DragAndDropLoot>().SetParentTransform(gameObject.transform, myDropType);
//        Debug.Log("UI spawning here " + type);
    }

    public void OnUIRemove(string id, OnDropType type)
    {
        //Debug.Log("heard event emitter for removal for type " + type + " and my type " +myDropType + " my gameobject name is " + gameObject.name);
        if (type != myDropType)
            return;
        // Debug.Log("heard event emitter for removal for type " + type + " and my type " +myDropType);
        GameObject toDestroy = null;
        var children = transform.GetComponentsInChildren<DragAndDropLoot>();
        foreach (var dd in children)
            if (dd.item.id == id)
                toDestroy = dd.gameObject;

        // Debug.Log("ui item to be destroyed " + toDestroy.GetComponent<DragAndDropLoot>().item);
        if (toDestroy == null)
        {
            Debug.Log("Something went wrong and we didnt find item in this UI");
            return;
        }

//        Debug.Log("UI Destroying here " + type + "   " + toDestroy);
        Destroy(toDestroy);
    }
}

public enum OnDropType
{
    Treasure,
    Inventory,
    AttackPanel,
    HeadSlot,
    ChestSlot,
    LegSlot,
    BootSlot,
    LeftHandSlot,
    RightHandSlot,
    RingSlot,
    AmuletSlot,
    Nulling,
    Selling,
    Stash
}