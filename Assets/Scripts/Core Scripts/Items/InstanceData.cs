using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class InstanceData
{
    public int highAttack;
    public int lowAttack;
    public int defense;
    public ItemRarity rarity;
    public string itemId;
    public int amountInThisStack;
    public OnDropType onDropType;
    public DamageType damageType;
    public List<ItemModBlock> modBlocks;
    

    [JsonConstructor]
    public InstanceData()
    {
    }
}