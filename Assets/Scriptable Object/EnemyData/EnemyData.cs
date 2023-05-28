using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData/Floor1/Basic")]
public class EnemyData : ScriptableObject
{
    public int health;
    public int damage;
    public Color color;
    public LootTable lootTable;
}
