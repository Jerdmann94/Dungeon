using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanelManager : MonoBehaviour
{
    public List<GameItem> runesInAttackPanel;

    public StaticReference attackOC;

    public RuneData starterRune;
    
    public GameObject attackSlotUI;
    // Start is called before the first frame update
    void Start()
    {
        attackOC.target = this.gameObject;
        runesInAttackPanel = new List<GameItem>();
        runesInAttackPanel.Add(new BasicRune(starterRune));
        foreach (Transform child in transform)
        {
            //Destroy(child.gameObject);
        }

        foreach (var item in runesInAttackPanel)
        {
            var ui =Instantiate(attackSlotUI, transform);
            ui.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.amountInThisStack.ToString();
            ui.transform.GetChild(1).GetComponent<Image>().sprite = item.sprite;
            ui.GetComponent<DragDropAttackItem>().currentRune = item;


        }
        
        
    }
    
}
