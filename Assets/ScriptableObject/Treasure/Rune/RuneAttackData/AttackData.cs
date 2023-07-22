
using UnityEngine;

[CreateAssetMenu(menuName = "RuneAttackData")]
public class AttackData : ScriptableObject
{
    public GameObject attackPrefab;
    public int rangeLowDamage;
    public int rangeHighDamage;
    public DamageType damageType;
    public Color color;
}
