using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRune : GameItem
{
    public int damage;
    public BasicRune(RuneData lootData)
    {
        amountInThisStack = Random.Range(1, lootData.maxSpawnAmount);
        sprite = lootData.sprite;
        maxStackAmount = lootData.stackAmount;
        name = lootData.name;
        damage = lootData.attackData.damage;

    }
}
