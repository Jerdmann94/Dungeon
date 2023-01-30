using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class GameItem
{
    
    public int maxStackAmount;
    public int amountInThisStack;
    public string name;
    public OnDropType onDropType;
    public string sprite;
    [SerializeField] public string id;


    public GameItem(string spritePath, int maxStackAmount, int amountInThisStack, string name, OnDropType onDropType)
    {
        
        this.maxStackAmount = maxStackAmount;
        this.amountInThisStack = amountInThisStack;
        this.name = name;
        this.onDropType = onDropType;
        id = Guid.NewGuid().ToString();
        sprite = spritePath;
    }

    public override string ToString()
    {
        string v = "";
        v += "Name = " + name;
        v += " Amount in this stack = " + amountInThisStack;
        v += " MaxStackAmount = " + maxStackAmount;
        v += " id = " + id;
        return v;
    }
}



