using UnityEngine;

public class BasicRune : GameItem
{
    public int damage;

    public BasicRune(RuneData lootData) : base(lootData.spritePath, lootData.maxSpawnAmount,
        Random.Range(1, lootData.maxSpawnAmount),
        lootData.name,
        OnDropType.AttackPanel)
    {
        damage = lootData.attackData.damage;
    }

    public override ToolTipData GetToolTip()
    {
        var t = new ToolTipData();

        t.rarity = rarity;
        t.dropType = onDropType;

        return t;
    }
}