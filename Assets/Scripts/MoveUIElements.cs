using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveUIElements : MonoBehaviour, IDragHandler,IPointerDownHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private bool locked;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button lockButton;
    public void OnDrag(PointerEventData eventData)
    {
        if (!locked)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        
    }

    public void ChangeLock()
    {
        locked = !locked;
        lockButton.GetComponent<Image>().color = locked ? Color.red : Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }
}
