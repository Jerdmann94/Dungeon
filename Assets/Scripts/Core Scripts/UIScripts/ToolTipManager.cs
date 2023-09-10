using System.Collections.Generic;
using System.Net;
using MyBox;
using TMPro;
using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager instance;
    public List<TMP_Text> names;
    public List<TMP_Text> values;
    public TMP_Text nameText;
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

    private void SetActiveToolTip(int i, bool b)
    {
      
        values[i].transform.parent.gameObject.SetActive(b);
        
    }
    public void ShowToolTip(ToolTipData toolTipData)
    {

        toolTipRect = toolTip.GetComponent<RectTransform>();
        toolTip.SetActive(true);
        nameText.SetText(toolTipData.name);
        nameText.color = GetRarityColor(toolTipData.rarity);
        for (int i = 0; i < 4; i++)
        {
            SetActiveToolTip(i, false);
        }
        for (int i = 0; i < 4; i++)
        {
            if (toolTipData.attack is not (null or ""))
            {
                SetActiveToolTip(i, true);
                names[i].SetText("Attack" );
                values[i].SetText(toolTipData.attack);
                i++;
            }

            if (toolTipData.defense != 0)
            {
                SetActiveToolTip(i, true);
                names[i].SetText("Defense" );
                values[i].SetText(toolTipData.defense.ToString());
                i++;
                
            }
            
            if (!toolTipData.dropType.ToString().IsNullOrEmpty())
            {
                SetActiveToolTip(i, true);
                names[i].SetText("Slot" );
                values[i].SetText(toolTipData.dropType.ToString());
                i++;
            }
            
            if (toolTipData.name == "Copper Coin")
            {
                SetActiveToolTip(i, true);
                names[i].SetText("Amount" );
                values[i].SetText(toolTipData.amountInThisStack.ToString());
            }

            break;
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