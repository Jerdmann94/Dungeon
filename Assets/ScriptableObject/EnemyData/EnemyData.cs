using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData/Floor1/Basic")]
public class EnemyData : ScriptableObject
{
    public int health;
    public int speed;
    public List<EnemyAttack> attacks;
    public Color color;
    public LootTable lootTable;
    public bool isRanged;
    [SerializeField]
    public Sprite sprite;

    
}

