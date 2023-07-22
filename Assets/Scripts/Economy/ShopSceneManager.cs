using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Unity.Services.Economy;
using Unity.Services.Economy.Model;



using UnityEngine;

public class ShopSceneManager : MonoBehaviour
 {
        const int k_EconomyPurchaseCostsNotMetStatusCode = 10504;

        public ShopView virtualShopSampleView;
        public InventoryHudView inventoryHudView;
        
        public async Task InitShop()
        {
            try
            {
               

                //Debug.Log($"Player id:{AuthenticationService.Instance.PlayerId}");

                // Economy configuration should be refreshed every time the app initializes.
                // Doing so updates the cached configuration data and initializes for this player any items or
                // currencies that were recently published.
                // 
                // It's important to do this update before making any other calls to the Economy or Remote Config
                // APIs as both use the cached data list. (Though it wouldn't be necessary to do if only using Remote
                // Config in your project and not Economy.)
                await EconomyManager.instance.RefreshEconomyConfiguration();
                if (this == null) return;

                EconomyManager.instance.InitializeVirtualPurchaseLookup();

              
                // Note: We want these methods to use the most up to date configuration data, so we will wait to
                // call them until the previous two methods (which update the configuration data) have completed.
                /*await Task.WhenAll(AddressablesManager.instance.PreloadAllEconomySprites(),
                    RemoteConfigManager.instance.FetchConfigs(),
                    EconomyManager.instance.RefreshCurrencyBalances());*/
                await Task.WhenAll(RemoteConfigManager.instance.FetchConfigs(),
                    EconomyManager.instance.RefreshCurrencyBalances(),EconomyManager.instance.RefreshInventory());
                if (this == null) return;

                // Read all badge addressables
                // Note: must be done after Remote Config values have been read (above).
                /*
                await AddressablesManager.instance.PreloadAllShopBadgeSprites(
                    RemoteConfigManager.instance.virtualShopConfig.categories);
                    */

                // Initialize all shops.
                // Note: must be done after all other initialization has completed (above).
                ShopManager.instance.Initialize();

                virtualShopSampleView.Initialize(ShopManager.instance.virtualShopCategories);

                var firstCategoryId = RemoteConfigManager.instance.virtualShopConfig.categories[0].id;
                if (!ShopManager.instance.virtualShopCategories.TryGetValue(
                    firstCategoryId, out var firstCategory))
                {
                    throw new KeyNotFoundException($"Unable to find shop category {firstCategoryId}.");
                }
                virtualShopSampleView.ShowCategory(firstCategory);

                Debug.Log("Initialization and sign in complete.");

                EnablePurchases();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
       

        void EnablePurchases()
        {
            virtualShopSampleView.SetInteractable();
        }

        public void OnCategoryButtonClicked(string categoryId)
        {
            var virtualShopCategory = ShopManager.instance.virtualShopCategories[categoryId];
            virtualShopSampleView.ShowCategory(virtualShopCategory);
        }

        public async Task OnPurchaseClicked(ShopItem virtualShopItem)
        {
            try
            {
                Debug.Log(virtualShopItem.id);
                var result = await EconomyManager.instance.MakeVirtualPurchaseAsync(virtualShopItem.id);
                if (this == null) return;

                await EconomyManager.instance.RefreshCurrencyBalances();
                if (this == null) return;

                
                Debug.Log(result.Rewards.Inventory[0].Id);
                Debug.Log(result.Rewards.Inventory[0].PlayersInventoryItemIds[0]);
                GetInventoryOptions options = new GetInventoryOptions
                {
                    ItemsPerFetch = 1,
                    PlayersInventoryItemIds = new List<string>() { result.Rewards.Inventory[0].PlayersInventoryItemIds[0] }
                };
                GetInventoryResult inventoryResult = await EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);

                List<PlayersInventoryItem> listOfItems = inventoryResult.PlayersInventoryItems;

                if (listOfItems.Count < 1)
                {
                    Debug.Log("purchased succeded but didnt get it from shop");
                    return;
                }
                var item = listOfItems[0];

                inventoryHudView.MakeItemUI(item);
                inventoryHudView.RefreshItemsInInventory();
                foreach(var cost in result.Costs.Currency)
                {
                    Debug.Log(cost);
                }
                foreach(var cost in result.Costs.Inventory)
                {
                    Debug.Log(cost);
                }
                foreach(var cost in result.Rewards.Currency)
                {
                    
                    Debug.Log(cost);
                }
                foreach(var cost in result.Rewards.Inventory)
                {
                    Debug.Log(cost);
                }
                
                

            }
            catch (EconomyException e)
            when (e.ErrorCode == k_EconomyPurchaseCostsNotMetStatusCode)
            {
               
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void OnGainCurrencyDebugButtonClicked()
        {
            try
            {
                await EconomyManager.instance.GrantDebugCurrency("COIN", 30);
                if (this == null) return;

                await EconomyManager.instance.RefreshCurrencyBalances();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
       

      
    }
