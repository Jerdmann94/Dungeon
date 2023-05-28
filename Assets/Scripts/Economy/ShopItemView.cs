using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Samples.VirtualShop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
    {
        public TextMeshProUGUI costAmount;
        ShopSceneManager m_VirtualShopSceneManager;
        ShopItem m_VirtualShopItem;

        public void Initialize(ShopSceneManager virtualShopSceneManager, ShopItem virtualShopItem)
        {
            m_VirtualShopSceneManager = virtualShopSceneManager;
            m_VirtualShopItem = virtualShopItem;

            var spr = Resources.Load<Sprite>(virtualShopItem.spriteString);
            Debug.Log(spr);
            GetComponent<Image>().sprite = spr;

            var cost = virtualShopItem.costs[0];
            
            costAmount.text = cost.amount.ToString();
            
        }

        Color GetColorFromString(string colorString)
        {
            if (ColorUtility.TryParseHtmlString(colorString, out var color))
            {
                return color;
            }

            return Color.white;
        }

        public async void OnPurchaseButtonClicked()
        {
            try
            {
                await m_VirtualShopSceneManager.OnPurchaseClicked(m_VirtualShopItem);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("mouse enter");
            ToolTipManager.instance.ShowToolTip(new ToolTipData
            {
                name = m_VirtualShopItem.id
            });
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToolTipManager.instance.HideToolTip();
        }
    }
