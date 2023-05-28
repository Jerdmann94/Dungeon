using TMPro;
using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;
    public TMP_Text nameText;
    public TMP_Text attack;
    public TMP_Text rarity;
    public TMP_Text slot;
    public TMP_Text type;
    public TMP_Text mod1;
    public TMP_Text mod2;
    public TMP_Text mod3;
    public TMP_Text mod4;
    public GameObject toolTip;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void ShowToolTip(ToolTipData toolTipData)
    {
        toolTip.SetActive(true);
        nameText.SetText(toolTipData.name);
//        Debug.Log(toolTipData.attack);
        if (toolTipData.attack is not (null or ""))
            attack.SetText("Attack = " + toolTipData.attack);
        else
            attack.SetText("Defense = " + toolTipData.defense);

        rarity.SetText(toolTipData.rarity.ToString());
        slot.SetText(toolTipData.dropType.ToString());

        switch (toolTipData.rarity)
        {
            case ItemRarity.Uncommon:
                rarity.color = Color.green;
                break;
            case ItemRarity.Rare:
                rarity.color = Color.blue;
                break;
            case ItemRarity.Epic:
                rarity.color = Color.magenta;
                break;
            case ItemRarity.Legendary:
                rarity.color = Color.yellow;
                break;
            default:
                rarity.color = Color.white;
                break;
        }

        type.SetText(toolTipData.damageType.ToString());

        if (toolTipData.modBlocks == null)
        {
            return;
        }
//        Debug.Log(toolTipData.modBlocks.Count);
        if (toolTipData.modBlocks.Count == 0)
        {
            mod1.gameObject.SetActive(false);
        }
        else
        {
            mod1.gameObject.SetActive(true);
            mod1.SetText(toolTipData.modBlocks[0].text);
        }

        if (toolTipData.modBlocks.Count < 2)
        {
            mod2.gameObject.SetActive(false);
        }
        else
        {
            mod2.gameObject.SetActive(true);
            mod2.SetText(toolTipData.modBlocks[1].text);
        }

        if (toolTipData.modBlocks.Count < 3)
        {
            mod3.gameObject.SetActive(false);
        }
        else
        {
            mod3.gameObject.SetActive(true);
            mod3.SetText(toolTipData.modBlocks[2].text);
        }

        if (toolTipData.modBlocks.Count < 4)
        {
            mod4.gameObject.SetActive(false);
        }
        else
        {
            mod4.gameObject.SetActive(true);
            mod4.SetText(toolTipData.modBlocks[3].text);
        }
    }

    public void HideToolTip()
    {
        toolTip.SetActive(false);
    }
}