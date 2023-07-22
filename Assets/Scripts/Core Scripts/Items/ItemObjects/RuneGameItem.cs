using UnityEngine;

public class RuneGameItem : GameItem
{
   
    public GameObject attackPrefab;
    public int rangeLowDamage;
    public int rangeHighDamage;
    public DamageType damageType;
    public Color color;

    public RuneGameItem(RuneData lootData, int i) : base(lootData.spritePath, i,
        lootData.name,
        OnDropType.AttackPanel,0)
    {
        attackPrefab = lootData.attackData.attackPrefab;
        rangeLowDamage = lootData.attackData.rangeLowDamage;
        rangeHighDamage = lootData.attackData.rangeHighDamage;
        damageType = lootData.attackData.damageType;
        color = lootData.attackData.color;
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();

        t.rarity = rarity;
        t.dropType = onDropType;

        return t;
    }
}