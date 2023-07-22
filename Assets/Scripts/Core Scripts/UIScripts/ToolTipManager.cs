using System.Net;
using MyBox;
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
    private RectTransform toolTipRect;
    public RectTransform canvasRect;
   

    private void Start()
    {
        toolTipRect = toolTip.GetComponent<RectTransform>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void Update()
    {
        
        Vector2 aPos = Input.mousePosition / canvasRect.localScale.x;
        if (aPos.x + toolTipRect.rect.width > canvasRect.rect.width)
        {
            aPos.x = canvasRect.rect.width - toolTipRect.rect.width;
        }

        if (aPos.y + toolTipRect.rect.height > canvasRect.rect.height)
        {
            aPos.y = canvasRect.rect.height - toolTipRect.rect.height;
        }

        toolTipRect.anchoredPosition = aPos;
        //toolTipRect.anchoredPosition = Input.mousePosition;
        //transform.position = Input.mousePosition;

    }

    public void ShowToolTip(ToolTipData toolTipData)
    {
//        Debug.Log("showing tooltip");
        toolTipRect = toolTip.GetComponent<RectTransform>();
        
        toolTip.SetActive(true);
        nameText.SetText(toolTipData.name);
//        Debug.Log(toolTipData.attack);
        if (toolTipData.attack is not (null or ""))
            attack.SetText("Attack = " + toolTipData.attack);
        else
            attack.SetText("Defense = " + toolTipData.defense);

        if (!toolTipData.rarity.ToString().IsNullOrEmpty())
        {
            rarity.SetText(toolTipData.rarity.ToString());
            rarity.color = GetRarityColor(toolTipData.rarity);
        }

        if (!toolTipData.dropType.ToString().IsNullOrEmpty())
        {
            slot.SetText(toolTipData.dropType.ToString());
        }

        type.SetText(!toolTipData.damageType.ToString().IsNullOrEmpty()
            ? toolTipData.damageType.ToString()
            : toolTipData.amountInThisStack.ToString());

//        Debug.Log(toolTipData.dropType);
        if (toolTipData.name == "Copper Coin")
        {
            slot.SetText(toolTipData.amountInThisStack.ToString());
            Debug.Log("Amount in this stack " + toolTipData.amountInThisStack.ToString());
        }
        
        if (toolTipData.modBlocks == null)
        {
            mod1.gameObject.SetActive(false);
            mod2.gameObject.SetActive(false);
            mod3.gameObject.SetActive(false);
            mod4.gameObject.SetActive(false);
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

    public static Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Uncommon:
                return Color.green;
                
            case ItemRarity.Rare:
                return Color.blue;
            
            case ItemRarity.Epic:
                return Color.magenta;
                
            case ItemRarity.Legendary:
                return Color.yellow;
                
            default:
                return Color.white;
        }
    }
    
}