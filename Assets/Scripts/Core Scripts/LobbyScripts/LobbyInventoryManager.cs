using System;
using System.Collections.Generic;
using Core_Scripts.UtilityScripts;
using UnityEngine;

public class LobbyInventoryManager : MonoBehaviour
{
    public List<GameItem> lootInInventory;
    public List<GameItem> lootInStash;
    public List<GameItem> lootInSelling;
    public LobbyStatBlock lobbyStatBlock;

    public Dictionary<OnDropType, GameItem> equipment;

    private void Start()
    {
        equipment = EquipmentUtil.InitEquip();
        lobbyStatBlock.statBlock.UpdateStats(equipment);
        lobbyStatBlock.statPanelManager.UpdateStatPanelText(lobbyStatBlock.statBlock);
//        Debug.Log("lobby inventory start" + equipment + " "  + lobbyStatBlock.statBlock);
    }

    //FOR PREGAME LOBBY ONLY
    public void RemoveItemFromInventoryAndEquipment(GameItem ddItem)
    {
        if (equipment == null)
        {
            equipment = EquipmentUtil.InitEquip();
            lootInInventory = new List<GameItem>();
        }

        var keys = equipment.Keys;
        var s = lootInStash.Find(item => item.id == ddItem.id);
        if (s != null) lootInStash.Remove(s);
        var i = lootInInventory.Find(item => item.id == ddItem.id);
        if (i != null) lootInInventory.Remove(i);
        var ss = lootInSelling.Find(item => item.id == ddItem.id);
        if (ss != null) lootInSelling.Remove(ss);

        var removeEquipmentItem = new GameItem();
        foreach (var key in keys)
        {
            if (equipment[key] == null) continue;
            if (equipment[key].id == ddItem.id) removeEquipmentItem = equipment[key];
        }

        equipment[removeEquipmentItem.onDropType] = null;
    }
}