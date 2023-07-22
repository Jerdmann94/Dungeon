using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanelManager : MonoBehaviour
{
    public List<RuneGameItem> runesInAttackPanel;

    public List<RuneData> runeDataList;
    public StaticReference attackOC;
    public GameObject attackSlotUI;

    // Start is called before the first frame update
    private void Start()
    {
        attackOC.Target = gameObject;
        runesInAttackPanel = new List<RuneGameItem>();
        //runesInAttackPanel.Add(new BasicRune(starterRune));
        foreach (var runeData in runeDataList)
        {
            runesInAttackPanel.Add(new RuneGameItem(runeData,5));
        }
        foreach (Transform child in transform) Destroy(child.gameObject);

        foreach (var item in runesInAttackPanel)
        {
            var ui = Instantiate(attackSlotUI, transform);
            ui.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.amountInThisStack.ToString();
            // ui.transform.GetChild(1).GetComponent<Image>().sprite = item.sprite;
            ui.transform.GetChild(1).GetComponent<Image>().color = item.color;
            //HAVE TO SET SPRITES THROUGH SPRITE STRINGS
            ui.GetComponent<DragDropAttackItem>().currentRune = item;
        }
    }
}