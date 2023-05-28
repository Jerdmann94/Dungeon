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
    public OnDropTypeStore preDragLocation;
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


    public void SetParentTransform(Transform ptransform, OnDropTypeStore location)
    {
        gameObject.transform.SetParent(ptransform);
        preDragLocation = location;
    }

    public void SetIconSprite(Sprite sprite)
    {
        childImage.sprite = sprite;
    }

    public void SetUpItem(GameItem i)
    {
        item = i;
    }
}