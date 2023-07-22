using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObj
{
    
    //DATA FROM EFFECT DATA
    public int damage;
    public int speed;
    public float halfLife;
    public int tickRate;
    public bool oneTimeEffect;
    
    //INSTANCE SPECIFIC DATA
    public bool alreadyTriggeredItsEffect;
    public float currentTickTimer;
    public float currentHalfLifeTimer;

    public EffectObj(int damage, int speed, float halfLife, int tickRate,bool oneTimeEffect)
    {
        this.oneTimeEffect = oneTimeEffect;
        this.damage = damage;
        this.speed = speed;
        this.halfLife = halfLife;
        this.tickRate = tickRate;
    }
}
