using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "coin",menuName = "Loot/Hands/Right/RightHandItem")]
public class RightHandData : LootData
{
    
    public int lowAttack;
    public int highAttack;
    public DamageType damageType;
    public override GameItem MakeGameContainer()
    {
  
        return new RightHandGameItem(this, lowAttack,highAttack, damageType);
    }

    
}
