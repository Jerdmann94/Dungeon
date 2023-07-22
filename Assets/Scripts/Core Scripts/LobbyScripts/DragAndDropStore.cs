using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropStore : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,
    IPointerEnterHandler, IPointerExitHandler

{
    public Texture2D texture;

    public Image image;

    public Image childImage;
    public ItemCustomData data;
    public OnDropType preDragLocation;
    public GameItem item;
    public string econInventoryId;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("begin drag");
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManager.instance.ShowToolTip(item.GetToolTip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.instance.HideToolTip();
    }


    private void OnDestroy()
    {
        image.raycastTarget = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void SetParentTransform(Transform ptransform, OnDropType location)
    {
        Debug.Log("setting parent for ui at :" +location + transform);
        gameObject.transform.SetParent(ptransform);
        preDragLocation = location;
    }

    public void SetIconSprite(Sprite sprite)
    {
        childImage.sprite = sprite;
    }

    public void SetUpItem(GameItem i)
    {
        transform.GetChild(0).GetComponent<Image>().color = ToolTipManager.GetRarityColor(i.rarity);
        item = i;
    }
}