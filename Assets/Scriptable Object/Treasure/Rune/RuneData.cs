using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Runes/Starter rune")]
public class RuneData : LootData
{
    public AttackData attackData;
    public override object MakeGameContainer()
    {
        throw new System.NotImplementedException();
    }
}
