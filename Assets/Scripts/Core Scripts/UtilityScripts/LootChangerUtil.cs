using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core_Scripts.UtilityScripts;
using MyBox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Collections;
using Unity.Services.Economy;
using UnityEngine;
using static UnityEngine.JsonUtility;

public class LootChangerUtil : MonoBehaviour
{
    //SHOULD BE USED IN GAME TO CONVERT NETWORK DATA TO GAME ITEMS
    public List<LootData> lootDatas;

    public static List<GameItem> LootExchanging(string changeJson)
    {
        var v = new List<GameItem>();
        var val = JsonConvert.DeserializeObject<List<GameItem>>(changeJson);
        var s = JArray.Parse(changeJson);
        foreach (var VARIABLE in s.Children().ToList()) v.Add(JsonToItem(VARIABLE.ToString()));
        //Debug.Log(changeJson.ToString());
        return v;
    }

    //SHOULD BE USED IN SHOP TO CONVERT SHOP STUFF TO GAME ITEMS
    public async Task<GameItem> StringLookUp(ItemCustomData cdata, InstanceData instanceData,
        string itemInventoryItemId)
    {
        bool needInstanceData = instanceData == null;
        
        //THIS LINE IS IMPORTANT
        //THIS NAME HAS TO MATCH DASHBOARD CONFIGURATION NAME FOR THIS ITEM
        Debug.Log(cdata.name);
        var item = lootDatas.Find(data => data.name == cdata.name);
        Debug.Log(item.name);
        var container = item.MakeGameContainer(1);
        container.id = itemInventoryItemId;
        //IF THIS IS A NEW ITEM, MAKE INSTANCE DATA
        if (needInstanceData)
        {
            container = await CreateInstanceDataForStoreItem(container);
        }
        else // AN ITEM ALREADY IN YOUR INVENTORY FROM BEFORE, GET THE DATA
        // NEED A WAY TO GET ATTACK / DEFENSE OF ITEMS HERE
        {
            container.amountInThisStack = instanceData.amountInThisStack;
            container.rarity = instanceData.rarity;
            container.modBlocks = instanceData.modBlocks;
            GetAttackForItem(container, instanceData);
        }
        var replace =  item.name.ToUpper().Replace(' ', '_');
        replace = replace.Replace("'", "_");
        container.sellID = "SELL" + replace;

        return container;
    }

    private void GetAttackForItem(GameItem container, InstanceData idata)
    {
        if (container.allowablePosition == OnDropType.RightHandSlot)
        {
            var rightHand = (RightHandGameItem)container;
            rightHand.highAttack = idata.highAttack;
            rightHand.lowAttack = idata.lowAttack;

        }
    }

    public static async Task<GameItem> CreateInstanceDataForStoreItem(GameItem gameItem)
    {
        var instanceData = new Dictionary<string, object>();
        instanceData.Add("Rarity", gameItem.rarity);
        instanceData.Add("ItemId", gameItem.id);
        instanceData.Add("AmountInThisStack", gameItem.amountInThisStack);
        instanceData.Add("modBlocks", gameItem.modBlocks);
        instanceData = AddInstanceSpecificData(gameItem, instanceData);


        var options = new AddInventoryItemOptions
        {
            PlayersInventoryItemId = gameItem.id,
            InstanceData = instanceData
        };
        await EconomyService.Instance.PlayerInventory.UpdatePlayersInventoryItemAsync(gameItem.id, options);


        return gameItem;
    }

    public static GameItem JsonToItem(string json)
    {
        if (json.IsNullOrEmpty())
        {
            return null;
        }
        var t = JObject.Parse(json)["allowablePosition"].ToObject<OnDropType>();
        GameItem g = null;
//        Debug.Log(t);

        switch (t)
        {
            case OnDropType.Inventory:
                g = JsonConvert.DeserializeObject<GameItem>(json);
                break;
            case OnDropType.Treasure:
                g = JsonConvert.DeserializeObject<GameItem>(json);
                break;
            case OnDropType.BootSlot:
                g = JsonConvert.DeserializeObject<BootGameItem>(json);
                break;
            case OnDropType.ChestSlot:
                g = JsonConvert.DeserializeObject<ChestGameItem>(json);
                break;
            case OnDropType.HeadSlot:
                g = JsonConvert.DeserializeObject<HelmetGameItem>(json);
                break;
            case OnDropType.LegSlot:
                g = JsonConvert.DeserializeObject<LegGameItem>(json);
                //Debug.Log(g + " " + g.modBlocks[0].text);
                break;
            case OnDropType.LeftHandSlot:
                g = JsonConvert.DeserializeObject<LeftHandGameItem>(json);
                break;
            case OnDropType.RightHandSlot:
                g = JsonConvert.DeserializeObject<RightHandGameItem>(json);
                break;
            case OnDropType.AmuletSlot:
                g = JsonConvert.DeserializeObject<AmuletGameItem>(json);
                break;
            case OnDropType.RingSlot:
                g = JsonConvert.DeserializeObject<RingGameItem>(json);
                break;
            case OnDropType.AttackPanel:
                break;
            case OnDropType.Nulling:
                break;
            case OnDropType.Selling:
                break;
            case OnDropType.Stash:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return g;
    }


    public static Dictionary<string, object> AddInstanceSpecificData(GameItem gameItem,
        Dictionary<string, object> instanceData)
    {
        switch (gameItem.allowablePosition)
        {
            case OnDropType.RightHandSlot:
                var rightHand = (RightHandGameItem)gameItem;
                instanceData.Add("lowAttack", rightHand.lowAttack);
                instanceData.Add("highAttack", rightHand.highAttack);
                instanceData.Add("damageType", rightHand.damageType);
                break;
            case OnDropType.BootSlot:
                var boot = (BootGameItem)gameItem;
                instanceData.Add("defense", boot.defense);
                instanceData.Add("damageType", boot.damageType);
                break;
            case OnDropType.ChestSlot:
                var c = (ChestGameItem)gameItem;
                instanceData.Add("defense", c.defense);
                instanceData.Add("damageType", c.damageType);
                break;
            case OnDropType.HeadSlot:
                var h = (HelmetGameItem)gameItem;
                instanceData.Add("defense", h.defense);
                instanceData.Add("damageType", h.damageType);
                break;
            case OnDropType.LegSlot:
                var l = (LegGameItem)gameItem;
                instanceData.Add("defense", l.defense);
                instanceData.Add("damageType", l.damageType);
                break;
            case OnDropType.LeftHandSlot:
                var lh = (LeftHandGameItem)gameItem;
                instanceData.Add("defense", lh.defense);
                instanceData.Add("damageType", lh.damageType);
                break;
            case OnDropType.AmuletSlot:
                instanceData.Add("damageType", DamageType.Physical);
                break;
            case OnDropType.RingSlot:
                instanceData.Add("damageType", DamageType.Physical);
                break;
            case OnDropType.Treasure:
                instanceData.Add("damageType", DamageType.Physical);
                break;
            case OnDropType.Inventory:
                instanceData.Add("damageType", DamageType.Physical);
                break;
            case OnDropType.AttackPanel:
                break;
            case OnDropType.Nulling:
                break;
            case OnDropType.Selling:
                break;
            case OnDropType.Stash:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return instanceData;
    }

    public static GameItem DownCastItem(GameItem gameItem)
    {
        switch (gameItem.allowablePosition)
        {
            case OnDropType.RightHandSlot:
                return (RightHandGameItem)gameItem;
            case OnDropType.BootSlot:
                return (BootGameItem)gameItem;
            case OnDropType.ChestSlot:
                return (ChestGameItem)gameItem;
            case OnDropType.HeadSlot:
                return (HelmetGameItem)gameItem;
            case OnDropType.LegSlot:
                return (LegGameItem)gameItem;
            case OnDropType.LeftHandSlot:
                return (LeftHandGameItem)gameItem;
            case OnDropType.AmuletSlot:
                return (AmuletGameItem)gameItem;
            case OnDropType.RingSlot:
                return (RingGameItem)gameItem;
        }
        Debug.Log("This should not be printing, item is supposed to return a real item");
        return null;
    }

    public static Dictionary<OnDropType, GameItem> ConvertJsonToEquipment(string equipmentJson)
    {
        var equipment = EquipmentUtil.InitEquip();
        var dropList = EquipmentUtil.InitEquip();
       
        
        foreach (var dropType in dropList)
        {
            
            var jobj = JObject.Parse(equipmentJson);
            if (jobj.ContainsKey(dropType.Key.ToString()))
            {
                var i = JsonToItem(jobj[dropType.Key.ToString()]?.ToString());
                equipment[dropType.Key] = i;
            }
        }
//        Debug.Log(" equipment json "+equipmentJson);
        return equipment;
    }
    


    public static FixedString512Bytes ItemToJson(GameItem item)
    {
        switch (item.allowablePosition)
        {
            case OnDropType.RightHandSlot:
                return ToJson((RightHandGameItem)item);
            case OnDropType.BootSlot:
                return ToJson((BootGameItem)item);
            case OnDropType.ChestSlot:
                return ToJson((ChestGameItem)item);
            case OnDropType.HeadSlot:
                return ToJson((HelmetGameItem)item);
            case OnDropType.LegSlot:
                return ToJson((LegGameItem)item);
            case OnDropType.LeftHandSlot:
                return ToJson((LeftHandGameItem)item);
            case OnDropType.AmuletSlot:
                return ToJson((AmuletGameItem)item);
            case OnDropType.RingSlot:
                return ToJson((RingGameItem)item);
        }
        return ToJson(item);
    }
}