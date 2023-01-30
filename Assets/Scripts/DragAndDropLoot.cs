using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropLoot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,IEndDragHandler,IDragHandler
{
    public Texture2D texture;

    public Image image;

    public GameItem item;

    public StaticReference inventoryOC;
    public StaticReference treasureOC;

    public OnDropType preDragLocation;
    /*public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift) && item.onDropType == OnDropType.AttackPanel)
        {
            MoveToAttackPanel();
        }
        if (!Input.GetMouseButtonDown(1) || !Input.GetKey(KeyCode.LeftShift)) return;
        switch (currentLocation)
        {
            case OnDropType.Inventory:
                MoveItemToInventory();
                return;
            case OnDropType.Treasure:
                
                MoveItemToTreasure();
                return;
        }
    }*/

    /*private void MoveToAttackPanel()
    {
        throw new NotImplementedException();
    }

    private void MoveItemToTreasure()
    {
        treasureOC.GetComponent<DropOnMe>().treasureScript.AddItemToChestServerRpc(item);
        inventoryOC.target.transform.parent.parent.parent
            .GetComponent<InventoryManager>().RemoveItemFromInventoryServerRpc(item);
        SetParentTransform(treasureOC.target.transform, OnDropType.Treasure);
        currentLocation = OnDropType.Treasure;
    }

    private void MoveItemToInventory()
    {
        treasureOC.GetComponent<DropOnMe>().treasureScript.RemoveItemFromChestServerRpc(item);
        inventoryOC.target.transform.parent.parent.parent
            .GetComponent<InventoryManager>().AddItemToInventoryServerRpc(item);
        SetParentTransform(inventoryOC.target.transform, OnDropType.Inventory);
        currentLocation = OnDropType.Inventory;
        
    }*/

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("begin drag");
        Cursor.SetCursor(texture,Vector2.zero, CursorMode.Auto);
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        
       
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        Cursor.SetCursor(null,Vector2.zero, CursorMode.Auto);
        /*if (!RaycastUtilities.PointerIsOverUI(Input.mousePosition)) return;
        var obj = RaycastUtilities.PointerIsOverUIObject(Input.mousePosition);
        Debug.Log(obj.name);
        Vector2 mousePos = Input.mousePosition;
        if (RectTransformUtility.RectangleContainsScreenPoint(
                treasureOC.GetComponent<RectTransform>()
                ,mousePos))
        {
            Debug.Log("Treasure");
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(
                     inventoryOC.GetComponent<RectTransform>()
                     ,mousePos))
        {
            Debug.Log("Inventory");
        }*/
        //check if mouse is over inventory space, if yes, place the item there
    }

    
    public void SetParentTransform(Transform ptransform, OnDropType location)
    {
        gameObject.transform.SetParent(ptransform);
        preDragLocation = location;
    }

    
}
