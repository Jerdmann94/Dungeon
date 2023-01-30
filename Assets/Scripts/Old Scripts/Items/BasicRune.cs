using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BasicRune : GameItem
{
    public int damage;
    public BasicRune(RuneData lootData) : base(lootData.spritePath,lootData.maxSpawnAmount, 
            Random.Range(1, lootData.maxSpawnAmount),
            lootData.name,
            OnDropType.AttackPanel)
    {
        damage = lootData.attackData.damage;
    }
    
}
