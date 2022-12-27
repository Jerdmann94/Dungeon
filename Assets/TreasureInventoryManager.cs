using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TreasureInventoryManager : MonoBehaviour
{
    public GameObject treasureParent;

    public GameObject inventorySlot;

    public StaticReference treasureOC;
    // Start is called before the first frame update
    void Start()
    {
        treasureOC.target = treasureParent;
        foreach (Transform child in treasureParent.transform)
        {
            Destroy(child);
        }

        var obj =Instantiate(inventorySlot, treasureParent.transform);
        obj.GetComponent<DragAndDropLoot>().currentLocation = OnDropType.Treasure;
    }
    

    public void FillUI(List<GameItem> treasureList)
    {
        foreach (Transform child in treasureParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in treasureList)
        {
            var ui =Instantiate(inventorySlot, treasureParent.transform);
            ui.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.amountInThisStack.ToString();
            ui.transform.GetChild(1).GetComponent<Image>().sprite = item.sprite;
            var dd = ui.GetComponent<DragAndDropLoot>();
            dd.item = item;
            dd.currentLocation = OnDropType.Treasure;
        }
        
    }
}
