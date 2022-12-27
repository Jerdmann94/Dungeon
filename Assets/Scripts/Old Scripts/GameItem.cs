using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem 
{
    public Sprite sprite;
    public int maxStackAmount;
    public int amountInThisStack;
    public string name;
    public LootType lootType;
}

public enum LootType
{
    Treasure,
    Rune
}
