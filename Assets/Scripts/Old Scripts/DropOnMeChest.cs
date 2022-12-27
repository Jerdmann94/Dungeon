using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropOnMeChest : MonoBehaviour, IDropHandler
{
    public TreasureScript treasureScript;
    
    
    


    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragAndDropLoot dragAndDropLoot = dropped.GetComponent<DragAndDropLoot>();
        dragAndDropLoot.SetParentTransform(this.gameObject.transform);
        treasureScript.AddItemToChestServerRpc(dragAndDropLoot.item);
        
    }
}
