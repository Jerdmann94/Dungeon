using UnityEngine;
using UnityEngine.EventSystems;

public class DropOnMeChest : MonoBehaviour, IDropHandler
{
    public TreasureScript treasureScript;


    public void OnDrop(PointerEventData eventData)
    {
    }
}