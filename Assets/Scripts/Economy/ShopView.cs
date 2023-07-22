using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Samples.VirtualShop;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    
    public ShopSceneManager virtualShopSceneManager;

    public GameObject comingSoonPanel;

    public Button inventoryButton;
    
    public Button gainCurrencyDebugButton;

    public GameObject itemsContainer;

    public GameObject categoryButtonsContainerGroup;
    public CategoryButton categoryButtonPrefab;
    
    public ShopItemView shopItemPrefab;
    
    List<CategoryButton> m_CategoryButtons = new List<CategoryButton>();
    void ClearContainer()
    {
        var itemsContainerTransform = itemsContainer.transform;
        for (var i = itemsContainerTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(itemsContainerTransform.GetChild(i).gameObject);
        }
    }
    public void SetInteractable(bool isInteractable = true)
    {
        //inventoryButton.interactable = isInteractable;
        gainCurrencyDebugButton.interactable = isInteractable;
    }
    public void Initialize(Dictionary<string, ShopCategory> virtualShopCategories)
    {
        foreach (var kvp in virtualShopCategories)
        {
            var categoryButtonGameObject = Instantiate(categoryButtonPrefab.gameObject, 
                categoryButtonsContainerGroup.transform);
            var categoryButton = categoryButtonGameObject.GetComponent<CategoryButton>();
            categoryButton.Initialize(virtualShopSceneManager, "category");
            m_CategoryButtons.Add(categoryButton);
        }
    }
    public void ShowCategory(ShopCategory virtualShopCategory)
    {
        ShowItems(virtualShopCategory);

        foreach (var categoryButton in m_CategoryButtons)
        {
            categoryButton.UpdateCategoryButtonUIState(virtualShopCategory.id);
        }

        comingSoonPanel.SetActive(!virtualShopCategory.enabledFlag);
    }
    void ShowItems(ShopCategory virtualShopCategory)
    {
        if (shopItemPrefab is null)
        {
            throw new NullReferenceException("Shop Item Prefab was null.");
        }

        ClearContainer();

        
        foreach (var virtualShopItem in virtualShopCategory.virtualShopItems)
        {
            var virtualShopItemGameObject = Instantiate(shopItemPrefab.gameObject, 
                itemsContainer.transform);
            
            //virtualShopItemGameObject.GetComponent<DragAndDropStore>().item;
//                Debug.Log(virtualShopItem);
            virtualShopItemGameObject.GetComponent<ShopItemView>().Initialize(
                virtualShopSceneManager, virtualShopItem);
        }
    }

}
