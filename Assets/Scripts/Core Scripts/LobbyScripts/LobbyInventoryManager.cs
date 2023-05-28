using System.Collections.Generic;
using UnityEngine;

public class LobbyInventoryManager : MonoBehaviour
{
    public List<GameItem> lootInInventory;
    public LobbyStatBlock lobbyStatBlock;

    public Dictionary<OnDropType, GameItem> equipment;

    //FOR PREGAME LOBBY ONLY
    public void RemoveItemFromInventoryAndEquipment(GameItem ddItem)
    {
        if (equipment == null)
        {
            equipment = new Dictionary<OnDropType, GameItem>();
            lootInInventory = new List<GameItem>();
        }

        var keys = equipment.Keys;
        var i = lootInInventory.Find(item => item.id == ddItem.id);
        // Debug.Log(i);
        if (i != null) lootInInventory.Remove(i);

        var removeEquipmentItem = new GameItem();
        foreach (var key in keys)
        {
            if (equipment[key] == null) continue;
            if (equipment[key].id == ddItem.id) removeEquipmentItem = equipment[key];
        }

        equipment[removeEquipmentItem.onDropType] = null;
    }
}