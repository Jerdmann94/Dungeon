using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveUIElements : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private static readonly int Collapse = Animator.StringToHash("Collapse");
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private bool locked;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button lockButton;
    [SerializeField] private GameObject expandPanel;

    private bool expanded = true;

    public void OnDrag(PointerEventData eventData)
    {
        if (!locked) rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }

    public void ChangeLock()
    {
        locked = !locked;
        lockButton.GetComponent<Image>().color = locked ? Color.red : Color.white;
    }

    public void ExpandUi()
    {
        expandPanel.SetActive(expanded);
        expanded = !expanded;
    }
}