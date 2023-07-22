
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TestLootGen : MonoBehaviour
{
    public TMP_Dropdown dropdownMenu;
    public TMP_Text text;
    public List<LootTable> tables;
    private int currentTable;
    public GameObject holdingPanel;
    public GameObject inventoryItemPrefab;
    private List<GameItem> items;
    public ShopSceneManager virtualShopSceneManager;
    //INFO FOR TEXT PANEL
    public GameObject textPanel;
    public GameObject textPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        dropdownMenu.onValueChanged.AddListener(SetText);
        tables = new List<LootTable>();
        dropdownMenu.options.Clear();
        PopulateList();
        SetupDropDown();
        
    }

    private void SetText(int dropdownInt)
    {
        text.SetText(tables[dropdownInt].name);
        currentTable = dropdownInt;
    }

    private void SetupDropDown()
    {
        foreach (var table in tables)
        {
            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(table.name));
        }
        
    }

    public void RollCurrentTable()
    {
        items = new List<GameItem>();
        items.Clear();
        while (textPanel.transform.childCount > 0)
        {
            DestroyImmediate(textPanel.transform.GetChild(0).gameObject);
        }
        while (holdingPanel.transform.childCount > 0)
        {
            DestroyImmediate(holdingPanel.transform.GetChild(0).gameObject);
        }
        
        for (var index = 0; index < tables[currentTable].loot.Count; index++)
        {
            var loot = tables[currentTable].loot[index];
            //IF CHANGE TO ROLL WASNT LOW ENOUGH, CONTINUE TO NEXT ITERATION
            if (!loot.RollOnTable(tables[currentTable].spawnChanceIn1000[index]))
                continue;
            //SPAWN ACTUAL GAME CONTAINER, HAVE TO GET AWAY FROM SCRIPTABLE OBJECT HERE
            var i = loot.MakeGameContainer(tables[currentTable].maxAmountSpawnable[index]);
            var pre = Instantiate(textPrefab,textPanel.transform);
            pre.transform.GetChild(0).GetComponent<TMP_Text>().SetText(i.name);
            pre.transform.GetChild(1).GetComponent<TMP_Text>().SetText((tables[currentTable].spawnChanceIn1000[index]/10).ToString());
            items.Add(i);
        }


        foreach (var gameItem in items)
        {
            var newInventoryItemGameObject = Instantiate(inventoryItemPrefab, holdingPanel.transform);
            var dd = newInventoryItemGameObject.GetComponent<DragAndDropStore>();
            dd.preDragLocation = OnDropType.Stash;
            dd.SetUpItem(gameItem);
            dd.SetIconSprite(SpriteUtil.GetSprite(gameItem.sprite));
            Debug.Log(gameItem.PrintStats());
                
        }
        
    }
    void PopulateList()
    {
        string[] assetNames = AssetDatabase.
            FindAssets(" Ltable", new[] { "Assets/ScriptableObject/Treasure/LootTable" });
        tables.Clear();
        Debug.Log(assetNames.Length);
        foreach (string SOName in assetNames)
        {
            var SOpath    = AssetDatabase.GUIDToAssetPath(SOName);
            var character = AssetDatabase.LoadAssetAtPath<LootTable>(SOpath);
            tables.Add(character);
        }
    }

    public async void GiveMeItems()
    {
        foreach (var item in items)
        {
            var result = await EconomyManager.instance.MakeVirtualPurchaseAsync(item.id);
            if (this == null) return;
        }
        
    }
}
