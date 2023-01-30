using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TreasureInventoryManager : NetworkBehaviour
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

       
    }
    

    public void FillUI(List<GameItem> treasureList)
    {
        ActivateTreasureUI();
        foreach (Transform child in treasureParent.transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log(treasureList);
        foreach (var item in treasureList)
        {
            var ui =Instantiate(inventorySlot, treasureParent.transform);
            ui.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.amountInThisStack.ToString();
            //Debug.Log(item.sprite);
            ui.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + item.sprite);
            //DOING SPRITES SOME OTHER WAY, WITH STRINGS TO FIND THEM
            var dd = ui.GetComponent<DragAndDropLoot>();
            dd.item = item;
            dd.preDragLocation = OnDropType.Treasure;
        }
        
    }

    private void ActivateTreasureUI()
    {
        Debug.Log(treasureParent.name);
        treasureParent.SetActive(true);
        
    }

    public void DeactivateTreasureUI()
    {
        treasureParent.SetActive(false);
    }

    [ClientRpc]
    public void DeactivateTreasureUIClientRpc(ClientRpcParams clientRpcParams)
    {
        DeactivateTreasureUI();
    }
    [ClientRpc]
    public void ActivateTreasureUIClientRpc(ClientRpcParams clientRpcParams)
    {
        ActivateTreasureUI();
    }
    
}
