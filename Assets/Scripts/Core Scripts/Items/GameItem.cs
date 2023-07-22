using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class GameItem : GameItemAbs
{
    
    public int amountInThisStack;
    public string name;
    public OnDropType onDropType;
    public OnDropType allowablePosition;
    public string sprite;
    [SerializeField] public string id;
    public string sellID;
    public ItemRarity rarity;
    public List<ItemModBlock> modBlocks;
    public int requiredItemLevel;
    public int value;

    [JsonConstructor]
    public GameItem()
    {
    }

    public GameItem(string spritePath, int amountInThisStack, string name, OnDropType onDropType,int value)
    {
        this.value = value;
        this.amountInThisStack = amountInThisStack;
        this.name = name;
        this.onDropType = onDropType;
        allowablePosition = onDropType;
        id = Guid.NewGuid().ToString();
        sprite = spritePath;
        if (allowablePosition == OnDropType.Inventory)
        {
            rarity = ItemRarity.Common;
            return;
        }
        modBlocks = new List<ItemModBlock>();
        rarity = RollRarity();

    }


    public ItemRarity RollRarity()
    {
        var ran = Random.Range(0, 101);
        var r = ItemRarity.Common;
        switch (ran)
        {
            case <= 70:
                break;
            case >= 71 and <= 85:
                r = ItemRarity.Uncommon;
                AddModifier(1);
                break;
            case >= 86 and <= 93:
                r = ItemRarity.Rare;
                AddModifier(2);
                break;
            case >= 94 and <= 98:
                r = ItemRarity.Epic;
                AddModifier(3);
                break;
            case >= 99 and <= 100:
                r = ItemRarity.Legendary;
                AddModifier(4);
                break;
        }

        return r;
    }

    private void AddModifier(int i)
    {
        if (onDropType == OnDropType.Inventory) return;
        var values = Enum.GetValues(typeof(ItemModifier));

        for (var j = 0; j < i; j++)
        {
            var rand = Random.Range(0, values.Length);
            var modblock = new ItemModBlock();
            switch ((ItemModifier)rand)
            {
                /*BonusHealth,
                BonusSpeed,
                PhysicalDamagePoints,
                MagicalDamagePoints,
                PhysicalArmor,
                MagicArmor,
                PhysicalDamagePercent,
                MagicDamagePercent,
                PhysicalResistance,
                MagicalResistance,
                Gestation,
                Tenacity,*/
                case ItemModifier.BonusHealth:
                    modblock.itemModifier = ItemModifier.BonusHealth;
                    modblock.amount = Random.Range(1, 10);
                    modblock.text = "Bonus Health: " + modblock.amount;
                    break;
                case ItemModifier.BonusSpeed:
                    modblock.itemModifier = ItemModifier.BonusSpeed;
                    modblock.amount = Random.Range(1, 2);
                    modblock.text = "Bonus Speed: " + modblock.amount;
                    break;
                case ItemModifier.PhysicalDamagePoints:
                    modblock.itemModifier = ItemModifier.PhysicalDamagePoints;
                    modblock.amount = Random.Range(1, 3);
                    modblock.text = "Physical Damage Points: " + modblock.amount;
                    break;
                case ItemModifier.MagicalDamagePoints:
                    modblock.itemModifier = ItemModifier.MagicalDamagePoints;
                    modblock.amount = Random.Range(1, 3);
                    modblock.text = "Magical Damage Points: " + modblock.amount;
                    break;
                case ItemModifier.PhysicalArmor:
                    modblock.itemModifier = ItemModifier.PhysicalArmor;
                    modblock.amount = Random.Range(1, 3);
                    modblock.text = "Physical Armor: " + modblock.amount;
                    break;
                case ItemModifier.MagicArmor:
                    modblock.itemModifier = ItemModifier.MagicArmor;
                    modblock.amount = Random.Range(1, 3);
                    modblock.text = "Magic Armor: " + modblock.amount;
                    break;
                case ItemModifier.PhysicalDamagePercent:
                    modblock.itemModifier = ItemModifier.PhysicalDamagePercent;
                    modblock.amount = Random.Range(1, 4);
                    modblock.text = "Physical Damage Percent: " + modblock.amount;
                    break;
                case ItemModifier.MagicDamagePercent:
                    modblock.itemModifier = ItemModifier.MagicDamagePercent;
                    modblock.amount = Random.Range(1, 4);
                    modblock.text = "Magic Damage Percent: " + modblock.amount;
                    break;
                case ItemModifier.PhysicalResistance:
                    modblock.itemModifier = ItemModifier.PhysicalResistance;
                    modblock.amount = Random.Range(1, 4);
                    modblock.text = "Physical Resistance: " + modblock.amount;
                    break;
                case ItemModifier.MagicalResistance:
                    modblock.itemModifier = ItemModifier.MagicalResistance;
                    modblock.amount = Random.Range(1, 4);
                    modblock.text = "Magic Resistance: " + modblock.amount;
                    break;
                case ItemModifier.Gestation:
                    modblock.itemModifier = ItemModifier.Gestation;
                    modblock.amount = Random.Range(1, 4);
                    modblock.text = "Gestation: " + modblock.amount;
                    break;
                case ItemModifier.Tenacity:
                    modblock.itemModifier = ItemModifier.Tenacity;
                    modblock.amount = Random.Range(1, 4);
                    modblock.text = "Tenacity: " + modblock.amount;
                    break;
                case ItemModifier.Might:
                    modblock.itemModifier = ItemModifier.Might;
                    modblock.amount = Random.Range(1, 2);
                    modblock.text = "Might: " + modblock.amount;
                    break;
                case ItemModifier.Wit:
                    modblock.itemModifier = ItemModifier.Wit;
                    modblock.amount = Random.Range(1, 2);
                    modblock.text = "Wit: " + modblock.amount;
                    break;
                case ItemModifier.Will:
                    modblock.itemModifier = ItemModifier.Will;
                    modblock.amount = Random.Range(1, 2);
                    modblock.text = "Will: " + modblock.amount;
                    break;
                case ItemModifier.Intelligence:
                    modblock.itemModifier = ItemModifier.MagicDamagePercent;
                    modblock.amount = Random.Range(1, 2);
                    modblock.text = "Intelligence: " + modblock.amount;
                    break;
            }

            modBlocks.Add(modblock);
        }
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();
        t.name = name;
        t.amountInThisStack = amountInThisStack;
        return t;
    }

    public string PrintStats()
    {

        var log = "Item name: " + name +
                  " Amount in this stack: " + amountInThisStack +
                  " DropType: " + onDropType +
                  " AllowablePosition: " + allowablePosition +
                  " Sprite: " + sprite +
                  " ID: " + id +
                  " SellID: " + sellID +
                  " Rarity: " + rarity +
                  " RequiredItemLevel: " + requiredItemLevel;

        if (modBlocks == null) return log;
        foreach (var VARIABLE in modBlocks)
        {
            log += " Modblock: " + VARIABLE.text;
        }

        return log;
    }
}

public class ToolTipData
{
    public string attack;
    public DamageType damageType;
    public int defense;
    public OnDropType dropType;
    public List<ItemModBlock> modBlocks;
    public string name;
    public ItemRarity rarity;
    public int amountInThisStack;

    
}

[Serializable]
public class ItemModBlock
{
    public int amount;
    public ItemModifier itemModifier;
    public string text;

    [JsonConstructor]
    public ItemModBlock()
    {
        
    }
}

// common = 70%
// uncommon = 15%
//rare = 8%
// epic == 5%
//legendary == 2%
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum DamageType
{
    Physical,
    Magical
}


public enum ItemModifier
{
    BonusHealth,
    BonusSpeed,
    PhysicalDamagePoints,
    MagicalDamagePoints,
    PhysicalArmor,
    MagicArmor,
    PhysicalDamagePercent,
    MagicDamagePercent,
    PhysicalResistance,
    MagicalResistance,
    Gestation,
    Tenacity,
    Might,
    Wit,
    Will,
    Intelligence
}