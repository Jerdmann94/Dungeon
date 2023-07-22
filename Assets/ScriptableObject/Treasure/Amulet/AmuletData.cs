using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "coin",menuName = "Loot/Amulet")]
public class AmuletData : LootData
{
    public override GameItem MakeGameContainer(int maxSpawnAmount)
    {
        return new AmuletGameItem(this);
    }
}
