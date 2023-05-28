using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RightHandGameItem : GameItem
{
    public int lowAttack;
    public int highAttack;
    public DamageType damageType;

    [JsonConstructor]
    public RightHandGameItem()
    {
    }

    public RightHandGameItem(LootData lootData, int lowAttack, int highAttack,DamageType damageType) : base(lootData.spritePath,
        lootData.maxSpawnAmount, lootData.stackAmount, lootData.name, OnDropType.RightHandSlot)
    {
        this.damageType = damageType;
        this.lowAttack = lowAttack;
        this.highAttack = highAttack;
        ScaleAttackBasedOnRarity();
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();
        t.name = name;
        t.attack = "" + lowAttack + "/" + highAttack;
        t.rarity = rarity;
        t.dropType = onDropType;
        t.damageType = damageType;
        t.modBlocks = modBlocks;
        return t;
    }
    
    private void ScaleAttackBasedOnRarity()
    {
        //LOW VARIANCE VARIABLE
        var lowVarianceAttack =  Random.Range(-1, 3);
        //HIGH VARIANCE VARIABLE;
        var highVarianceAttack = Random.Range(0, 3);
        var lowRarityAttack = 0;
        var highRarityAttack = 0;
        switch (rarity)
        {
            case ItemRarity.Common:
                break;
            case ItemRarity.Uncommon:
                lowRarityAttack = 2;
                highRarityAttack  = 2;
                break;
            case ItemRarity.Rare:
                lowRarityAttack = 3;
                highRarityAttack  = 4;
                break;
            case ItemRarity.Epic:
                lowRarityAttack = 4;
                highRarityAttack  = 6;
                break;
            case ItemRarity.Legendary:
                lowRarityAttack = 5;
                highRarityAttack = 8;
                break;
        }
      Debug.Log( lowAttack + " low Rarity attack: " +
                lowRarityAttack + " low variance attack: " +
                 lowVarianceAttack);
      Debug.Log( highAttack + " high Rarity attack: " +
                 highRarityAttack + " high variance attack: " +
                 highVarianceAttack +
                 " Rarity: " + rarity);
      lowAttack += (lowRarityAttack + lowVarianceAttack);
      highAttack += (highRarityAttack + highVarianceAttack);
    }
}