using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenObject : MonoBehaviour
{
   public RectTransform uiElement;
   public TreasureScript treasureScript;
   public TreasureInventoryManager treasureInventoryManager;
   public StaticReference treasureOC;
   public StaticReference inventoryOC;

   private void Start()
   {
      uiElement = GameObject.FindWithTag("TreasurePanel").GetComponent<RectTransform>();
      treasureInventoryManager = uiElement.GetComponent<TreasureInventoryManager>();
   }

   public void OnMouseOver(){
      
      if (Input.GetMouseButtonDown(1))
      {

         inventoryOC.target.GetComponent<DropOnMe>().treasureScript = treasureScript;
         treasureOC.target.GetComponent<DropOnMe>().treasureScript = treasureScript;
         uiElement.gameObject.SetActive(true);
         treasureInventoryManager.FillUI(treasureScript.lootInThisChest);

      }
   }
   public void CloseUI()
   {
      uiElement.gameObject.SetActive(false);
   }
}
