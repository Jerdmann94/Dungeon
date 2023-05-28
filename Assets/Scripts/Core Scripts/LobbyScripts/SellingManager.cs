using System.Collections.Generic;
using Unity.Services.Economy;
using UnityEngine;

public class SellingManager : MonoBehaviour
{
    public GameObject sellingOC;
    public EconomyManager economyManager;


    public async void SellAllInBox()
    {
        foreach (var dd in sellingOC.GetComponentsInChildren<DragAndDropStore>())
        {
            var option = new MakeVirtualPurchaseOptions();
            option.PlayersInventoryItemIds = new List<string>();
            option.PlayersInventoryItemIds.Add(dd.econInventoryId);
            Debug.Log(dd.econInventoryId);
            var result = await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(dd.item.sellID, option);
        }

        foreach (Transform child in sellingOC.transform) Destroy(child.gameObject);
        //NEED TO UPDATE UI TO SHOW NEW COINS IN INVENTORY
        await economyManager.RefreshCurrencyBalances();
    }
}