using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Services.Economy;
using UnityEngine;

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
        var item = lootDatas.Find(data => data.name == cdata.name);
        var container = item.MakeGameContainer();
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

        container.sellID = "SELL" + item.name.ToUpper();

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
        var t = JObject.Parse(json)["allowablePosition"].ToObject<OnDropType>();
        GameItem g = null;
        //Debug.Log(t);

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
                break;
            case OnDropType.LeftHandSlot:
                g = JsonConvert.DeserializeObject<LeftHandGameItem>(json);
                break;
            case OnDropType.RightHandSlot:
                g = JsonConvert.DeserializeObject<RightHandGameItem>(json);
                break;
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
        }
        Debug.Log("This should not be printing, item is supposed to return a real item");
        return null;
    }
}