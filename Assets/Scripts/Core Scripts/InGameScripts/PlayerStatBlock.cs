using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class PlayerStatBlock
{
    //CORE STATS BASE 10
    public int might = 10;
    public int wit = 10;
    public int will = 10;
    public int intelligence = 10;

    //Armor core stats
    public int armorMight;
    public int armorWit;
    public int armorWill;
    public int armorIntelligence;

    //MINOR STATS
    //--------------------------------BASE STATS ---------------------------------------
    //BASE 100 - might
    public NetworkVariable<int> maxHealth = new(100);

    //BASE 100 - might
    public NetworkVariable<int> currentHealth = new(100);

    //BASE 10 - wit - for each 3 wit +1 speed
    public float speedStat = 10;

    //BASE .5
    public float movementSpeed = .5f;

    //BASE 0
    public float movementCD;

    //BASE 0 - might - foreach might +1 phys
    public int physicalDamagePoints;

    //BASE 0 - int - foreach int +1 magic
    public int magicalDamagePoints;

    //BASE 0 
    public int physicalArmor;

    //BASE 0
    public int magicArmor;

    //BASE 0 - wit - foreach 3 wit +1 phys per
    public float physicalDamagePercent;

    //BASE 0 - will - foreach 2 will +1.5 magic per
    public float magicalDamagePercent;

    //BASE 0
    public float physicalResistance;

    //BASE 0 
    public float magicResistance;

    //BASE 5 log 1000 for time of food - int - foreach int + .25 gestation
    public float gestation = 5;

    //BASE 1 - will - foreach will +.25 tenacity
    public float tenacity = 1;

   

    // --------------------------MINOR STATS FROM ARMOR----------------------------------
    public int armorBonusHealth;
    public float armorSpeedStat;
    public int armorPhysicalDamagePoints;
    public int armorMagicalDamagePoints;
    public int armorPhysicalArmor;
    public int armorMagicArmor;
    public float armorPhysicalDamagePercent;
    public float armorMagicalDamagePercent;
    public float armorPhysicalResistance;
    public float armorMagicResistance;
    public float armorGestation;
    public float armorTenacity;
    
    //stats from armor that will be converted into damage range and damage reduction
    public int armorDefense;

    //attack rating that will be converted into damage
    //THIS ISNT IN USE YET
    public int attackRating;
    
    //THESE TWO NUMBERS SIGNIFY THE ATTACK RANGE OF THE EQUIPPED WEAPON. 
    public int lowAttack;
    public int highAttack;
    
    [JsonConstructor]
    public PlayerStatBlock()
    {
    }

    public void InitStats()
    {
    }

    public void UpdateStats(Dictionary<OnDropType, GameItem> equipment)
    {
        //Debug.Log(equipment);
        ResetArmorStats();
        if (equipment != null)
            
            foreach (var kp in equipment)
            {
                var item = kp.Value;
                if (kp.Value == null)
                {
                    continue;
                }
                Debug.Log(item.allowablePosition);
                //GET DEFENSE AND ATTACK FROM WEAPONS AND ARMOR AND APPLY THEM TO ARMOR DEFENSE/ ARMOR attack high/low
                switch (kp.Key)
                {
                    case OnDropType.HeadSlot:
                        armorDefense += ((HelmetGameItem)item).defense;
                        break;
                    case OnDropType.ChestSlot:
                        armorDefense += ((ChestGameItem)item).defense;
                        break;
                    case OnDropType.LegSlot:
                        armorDefense += ((LegGameItem)item).defense;
                        break;
                    case OnDropType.LeftHandSlot:
                        armorDefense += ((LeftHandGameItem)item).defense;
                        break;
                    case OnDropType.RightHandSlot:
                        lowAttack = ((RightHandGameItem)item).lowAttack;
                        highAttack = ((RightHandGameItem)item).highAttack;
                        break;
                    case OnDropType.BootSlot:
                        armorDefense += ((BootGameItem)item).defense;
                        break;
                }
                //DOING ARMOR MODIFIERS
                if (kp.Value?.modBlocks == null)
                    //                Debug.Log("Mod blocks are null");
                    continue;
                foreach (var modBlock in kp.Value.modBlocks)
                    //                Debug.Log(modBlock.text + modBlock.amount);
                    switch (modBlock.itemModifier)
                    {
                        case ItemModifier.BonusHealth:
                            armorBonusHealth += modBlock.amount;
                            break;
                        case ItemModifier.BonusSpeed:
                            armorSpeedStat += modBlock.amount;
                            break;
                        case ItemModifier.PhysicalDamagePoints:
                            armorPhysicalDamagePoints += modBlock.amount;
                            break;
                        case ItemModifier.MagicalDamagePoints:
                            magicalDamagePoints += modBlock.amount;
                            break;
                        case ItemModifier.PhysicalArmor:
                            armorPhysicalArmor += modBlock.amount;
                            break;
                        case ItemModifier.MagicArmor:
                            armorMagicArmor += modBlock.amount;
                            break;
                        case ItemModifier.PhysicalDamagePercent:
                            armorPhysicalArmor += modBlock.amount;
                            break;
                        case ItemModifier.MagicDamagePercent:
                            armorMagicalDamagePercent += modBlock.amount;
                            break;
                        case ItemModifier.PhysicalResistance:
                            armorPhysicalResistance += modBlock.amount;
                            break;
                        case ItemModifier.MagicalResistance:
                            armorMagicResistance += modBlock.amount;
                            break;
                        case ItemModifier.Gestation:
                            armorGestation += modBlock.amount;
                            break;
                        case ItemModifier.Tenacity:
                            armorTenacity += modBlock.amount;
                            break;
                        case ItemModifier.Might:
                            armorMight += modBlock.amount;
                            break;
                        case ItemModifier.Wit:
                            armorWit += modBlock.amount;
                            break;
                        case ItemModifier.Will:
                            armorWill += modBlock.amount;
                            break;
                        case ItemModifier.Intelligence:
                            armorIntelligence += modBlock.amount;
                            break;
                    }
            }

        
        //THEN DO CORE STAT CALCULATIONS
        //MIGHT
        maxHealth.Value = 100 + armorBonusHealth + GetMight() * 5;
        //Debug.Log("armor bonus health " + armorBonusHealth +" might " + might + " armor might " + armorMight) ;
        if (currentHealth.Value > maxHealth.Value) currentHealth.Value = maxHealth.Value;
        physicalDamagePoints = GetMight();
        //WIT
        speedStat = 10 + GetWit() / 3;
        physicalDamagePercent = 0 + GetWit() / 3;
        //WILL
        magicalDamagePercent = GetWill() * .75f;
        tenacity = 1 + GetWill() * .25f;
        //INT
        magicalDamagePoints = GetIntelligence();
        gestation = 5 + GetIntelligence() * .25f;

        //Debug.Log("right before send stats to client");
        var sb = JsonConvert.SerializeObject(this);
//        Debug.Log(sb);
    }

    public void RecieveStatsFromServer(string stat)
    {
        Debug.Log("Updating stats on client");
        var s = JsonConvert.DeserializeObject<PlayerStatBlock>(stat);
        might = s.might;
        wit = s.wit;
        will = s.will;
        intelligence = s.intelligence;
        armorMight = s.armorMight;
        armorWit = s.armorWit;
        armorWill = s.armorWill;
        armorIntelligence = s.armorIntelligence;
        speedStat = s.speedStat;
        physicalDamagePoints = s.physicalDamagePoints;
        magicalDamagePoints = s.magicalDamagePoints;
        physicalArmor = s.physicalArmor;
        magicArmor = s.magicArmor;
        physicalDamagePercent = s.physicalDamagePercent;
        magicalDamagePercent = s.magicalDamagePercent;
        physicalResistance = s.physicalResistance;
        magicResistance = s.magicResistance;
        gestation = s.gestation;
        tenacity = s.tenacity;
        armorBonusHealth = s.armorBonusHealth;
        armorSpeedStat = s.armorSpeedStat;
        armorPhysicalDamagePoints = s.armorPhysicalDamagePoints;
        armorMagicalDamagePoints = s.armorMagicalDamagePoints;
        armorPhysicalArmor = s.armorPhysicalArmor;
        armorMagicArmor = s.armorMagicArmor;
        armorPhysicalDamagePercent = s.armorPhysicalDamagePercent;
        armorMagicalDamagePercent = s.armorMagicalDamagePercent;
        armorPhysicalResistance = s.armorPhysicalResistance;
        armorMagicResistance = s.armorMagicResistance;
        armorGestation = s.armorGestation;
        armorTenacity = s.armorTenacity;
        lowAttack = s.lowAttack;
        highAttack = s.highAttack;
        attackRating = s.attackRating;
    }


    private void ResetArmorStats()
    {
        armorMight = 0;
        armorWit = 0;
        armorWill = 0;
        armorIntelligence = 0;
        armorBonusHealth = 0;
        armorSpeedStat = 0;
        armorPhysicalDamagePoints = 0;
        armorMagicalDamagePoints = 0;
        armorMagicArmor = 0;
        armorPhysicalDamagePercent = 0;
        armorMagicalDamagePercent = 0;
        armorPhysicalResistance = 0;
        armorMagicResistance = 0;
        armorGestation = 0;
        armorTenacity = 0;
        lowAttack = 0;
        highAttack = 0;
        attackRating = 0;
        armorDefense = 0;
    }

    public void UpdateMoveCooldown()
    {
        if (movementCD > 0) movementCD -= Time.deltaTime;
    }

    public bool CheckMoveCoolDown()
    {
        return movementCD > 0;
    }

    public void ResetMoveCoolDown()
    {
        movementCD += movementSpeed;
    }

    public float GetSpeedStat()
    {
        return armorSpeedStat + speedStat;
    }

    public int GetIntelligence()
    {
        return intelligence + armorIntelligence;
    }

    public float GetWill()
    {
        return will + armorWill;
    }

    public int GetWit()
    {
        return wit + armorWit;
    }

    public int GetMight()
    {
        return might + armorMight;
    }

    public int GetPhysicalDamagePoints()
    {
        return physicalDamagePoints + armorPhysicalDamagePoints;
    }

    public int GetMagicalDamagePoints()
    {
        return magicalDamagePoints + armorMagicalDamagePoints;
    }

    public int GetPhysicalArmor()
    {
        return physicalArmor + armorPhysicalArmor;
    }

    public int GetMagicArmor()
    {
        return magicArmor + armorMagicArmor;
    }

    public float GetPhysicalDamagePercent()
    {
        return physicalDamagePercent + armorPhysicalDamagePercent;
    }

    public float GetMagicDamagePercent()
    {
        return magicalDamagePercent + armorMagicalDamagePercent;
    }

    public float GetPhysicalResistance()
    {
        return physicalResistance + armorPhysicalResistance;
    }

    public float GetMagicResistance()
    {
        return magicResistance + armorMagicResistance;
    }

    public float GetGestation()
    {
        return gestation + armorGestation;
    }

    public float GetTenacity()
    {
        return tenacity + armorTenacity;
    }

    public int GetArmorRating()
    {
        return armorDefense;
    }

    public int GetAttackRating()
    {
        return attackRating;
    }

    public string GetAttackRangeText()
    {
        return lowAttack + " - " + highAttack;
    }
}