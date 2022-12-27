using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventoryManager : MonoBehaviour

{
    public List<GameItem> lootInInventory;
    public GameObject inventoryParent;

    public GameObject inventorySlot;

    public StaticReference inventoryOC;
    // Start is called before the first frame update
    void Start()
    {
        inventoryOC.target = inventoryParent;
        lootInInventory = new List<GameItem>();
        foreach (Transform child in inventoryParent.transform)
        {
            Destroy(child);
        }

        var obj =Instantiate(inventorySlot, inventoryParent.transform);
        obj.GetComponent<DragAndDropLoot>().currentLocation = OnDropType.Inventory;
    }


    [ServerRpc]
    public void AddItemToInventoryServerRpc(GameItem item)
    {
        lootInInventory.Add(item);
    }
    [ServerRpc]
    public void RemoveItemFromInventoryServerRpc(GameItem item)
    {
        lootInInventory.Remove(item);
    }

    
}
