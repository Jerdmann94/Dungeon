using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot Tables/Basic", fileName = "LootTable")]
public class LootTable : ScriptableObject
{
    public List<LootData> loot;
    public List<int> spawnChanceIn1000;
    public List<int> maxAmountSpawnable;

}
