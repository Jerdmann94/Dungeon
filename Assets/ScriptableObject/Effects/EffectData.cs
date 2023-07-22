using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Effects")]
public class EffectData : ScriptableObject
{
    public int damage;
    public int speed;
    public float halfLife;
    public int tickRate;
    public bool oneTimeEffect;
    public GameObject particlePrefab;
}
