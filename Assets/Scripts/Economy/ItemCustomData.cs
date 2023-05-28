using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemCustomData
{
    public int amountInThisStack;
    public int maxStackAmount;
    public string sprite;
    public OnDropTypeStore onDropType;
    public string name;
    public DamageType damageType;
}

