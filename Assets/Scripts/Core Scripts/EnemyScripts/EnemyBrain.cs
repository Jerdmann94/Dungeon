using System;
using System.Collections.Generic;
using Core_Scripts.EnemyScripts;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class EnemyBrain : NetworkBehaviour, IEffectable
{
    public Slider healthSlider;
    private int maxHealth;
    public List<PlayerThreat> threatTable;
    public string id;
    public EnemyMovementController enemyMovementController;
    public EnemyAttackController enemyAttackController;
    public EnemyMovementTimer enemyMovementTimer;
    public StaticReference textManager;
    private List<EffectObj> effects = new();
    public GameTreasureSpawnEvent gameTreasureSpawnEvent;
    [SerializeField] private LootTable lootTable;
    private readonly NetworkVariable<int> currentHealth = new();
    private NetworkVariable<FixedString512Bytes> dataNV = new ();
    public EnemyDataCollection enemyDataCollection;
    public SpriteRenderer spriteObj;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsClient)
            return;
        if (!collision.gameObject.CompareTag("Attack"))
            return;
        var attackScript = collision.gameObject.GetComponent<AttackScript>();
        HealthChange(-attackScript.damage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAwake") && !gameObject.CompareTag("TestDummy")) ;
        //Debug.Log("Exit Trigger Detected");
    }

    public void SetUpEnemyData(EnemyData enemyData)
    {
        if (enemyData == null)
        {
            Debug.Log("Generic enemy data, did not get a special tile");
            return;
        }

        dataNV.Value = enemyData.name;
        this.gameObject.name = enemyData.name;
        maxHealth = enemyData.health;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        currentHealth.Value = maxHealth;
        spriteObj.color = enemyData.color;
        spriteObj.sprite = enemyData.sprite;
        lootTable = enemyData.lootTable;
        enemyMovementController.Speed = enemyData.speed;
        enemyMovementTimer.isRanged = enemyData.isRanged;
        enemyMovementTimer.GetInitTimer(enemyData.speed);
        enemyAttackController.Init(enemyData.attacks);
        effects = new List<EffectObj>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
        {
            return;
        }
        currentHealth.OnValueChanged += (value, newValue) =>
        {
            Debug.Log(value + " new value " + newValue);
            healthSlider.value = newValue;
        };
        SetupClientData();
        base.OnNetworkSpawn();
    }

    private void SetupClientData()
    {
       var enemyData = enemyDataCollection.GetEnemyData(dataNV.Value.ToString());
       gameObject.name = enemyData.name;
       spriteObj.color = enemyData.color;
       spriteObj.sprite = enemyData.sprite;
       healthSlider.value = enemyData.health;
       healthSlider.maxValue = enemyData.health;

    }

    public void HealthChange(int i)
    {
//        Debug.Log("health change value " + i + " CURRENT HEALTH " + currentHealth.Value + " maxhealth " + maxHealth +" "+ gameObject.name);
        var textMan = textManager.Target.GetComponent<TextManager>();
        //THIS COLOR THING ISNT WORKING, NO TEXT IS RED OR GREEN
        textMan.CreatePopUp(transform.position, i, i > 0 ? Color.green : Color.red);
        if (i + currentHealth.Value > maxHealth)
        {
            currentHealth.Value = maxHealth;
        }
        else
        {
            currentHealth.Value += i;
        }
        healthSlider.value = currentHealth.Value;
        if (currentHealth.Value <= 0) ServerSideDespawn();
    }

    [ServerRpc]
    private void DeSpawnServerRpc()
    {
        ServerSideDespawn();
    }

    private void ServerSideDespawn()
    {
        if (!IsServer)
            return;
        SpawnTreasureObject();
        GetComponent<NetworkObject>().Despawn(this);
    }


    private void SpawnTreasureObject()
    {
        var position = transform.position;
        var pos = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), -1);
        gameTreasureSpawnEvent.TriggerEvent(pos, lootTable);

        /*var position = transform.position;
        var pos = new Vector3(position.x, position.y, position.z);
        var treasure = Instantiate(treasurePrefab, position, Quaternion.identity);
        treasure.GetComponent<NetworkObject>().Spawn();*/
    }

    private void Update()
    {
        if (!IsServer) return;
        HandleEffect();
    }

    public void AddEffect(EffectObj effect)
    {
        effects.Add(effect);
    }

    public void HandleEffect()
    {

        var effectsToRemove = new List<EffectObj>();
        
        if (effects.Count == 0) return;

        foreach (var effect in effects)
        {
            //DEAL WITH TIME PASSING FIRST
            effect.currentHalfLifeTimer += Time.deltaTime;
            effect.currentTickTimer += Time.deltaTime;
            if (effect.currentHalfLifeTimer > effect.halfLife)
            {
                effectsToRemove.Add(effect);
                continue;
            }

            //the only way this can be true is if it is a one time and it already happened
            if (effect.alreadyTriggeredItsEffect) continue;
            //DO TICK
            if (effect.currentTickTimer <= effect.tickRate) continue;
           // if (effect.oneTimeEffect && (!effect.oneTimeEffect || effect.alreadyTriggeredItsEffect)) continue;
            if (effect.damage != 0)
            {
                HealthChange(effect.damage);
            }

            if (effect.speed != 0)
            {
                enemyMovementController.buffSpeed += effect.speed;
                enemyMovementTimer.SetBuffTimer(effect.speed);
            }
                
            if (effect.oneTimeEffect)
            {
                effect.alreadyTriggeredItsEffect = true;
            }

        }

        foreach (var effect in effectsToRemove)
        {
            RemoveEffect(effect);
        }
        

    }

    public void RemoveEffect(EffectObj effect)
    {
        if (effect.speed !=0)
        {
            enemyMovementController.buffSpeed -= effect.speed;
            enemyMovementTimer.buffTimer = 0;
        }
        effects.Remove(effect);
    }
}