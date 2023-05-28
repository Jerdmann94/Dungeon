using System;
using System.Collections.Generic;
using Core_Scripts.EnemyScripts;
using Pathfinding;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemy : NetworkBehaviour
{
    public Slider healthSlider;
    public int maxHealth;
    public List<PlayerThreat> threatTable;
    public string id;
    public EnemyMovementController enemyMovementController;
    public StaticReference textManager;
    
    public ListContainer listContainer; // ONLY HERE TO TEST PATHING

    public GameTreasureSpawnEvent gameTreasureSpawnEvent;
    [SerializeField] private LootTable lootTable;
    private BasicEnemyMelee basicEnemyMelee;

    private readonly NetworkVariable<int> currentHealth = new();

    private void Awake()
    {
        //THIS IS JUST FOR TESTING PATHFINDING, THIS SHOULD BE MOVED BACK TO MOBSPAWNER
        //AND MOB SPAWNING NEEDS TO BE ADJUSTED SO MOBS SPAWN ON RUN NOT PREBAKED
        var newEnemyId = new Guid().ToString();
        Debug.Log(newEnemyId);
        id = newEnemyId;
        listContainer.enemyLookUp.Add(newEnemyId, this);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        currentHealth.Value = maxHealth;
        basicEnemyMelee = gameObject.GetComponent<BasicEnemyMelee>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsClient)
            return;
        if (collision.CompareTag("EnemyAwake") && !gameObject.CompareTag("TestDummy"))
        {
            var o = collision.gameObject;
            basicEnemyMelee.Target = o;
        }

        if (!collision.gameObject.CompareTag("Attack"))
            return;
        var attackScript = collision.gameObject.GetComponent<AttackScript>();
        var textMan = textManager.target.GetComponent<TextManager>();
//        Debug.Log(attackScript + " " + textMan);
        textMan.testMethod();
        textMan.CreatePopUp(transform.position, attackScript.damage, Color.red);
        // Debug.Log("Damage: " + attackScript.damage );
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
            Debug.Log("no enemy data");
            return;
        }
        basicEnemyMelee = gameObject.GetComponent<BasicEnemyMelee>();
        maxHealth = enemyData.health;
        basicEnemyMelee.damage = enemyData.damage;
        GetComponent<SpriteRenderer>().color = enemyData.color;
        lootTable = enemyData.lootTable;
        Debug.Log("enemy data color " + enemyData.color);
    }

    public override void OnNetworkSpawn()
    {
        currentHealth.OnValueChanged += (value, newValue) =>
        {
            //Debug.Log(value + " new value " + newValue);
            healthSlider.value = newValue;
        };
        base.OnNetworkSpawn();
    }

    private void HealthChange(int i)
    {
        currentHealth.Value += i;
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
        var pos = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        gameTreasureSpawnEvent.TriggerEvent(pos, lootTable);

        /*var position = transform.position;
        var pos = new Vector3(position.x, position.y, position.z);
        var treasure = Instantiate(treasurePrefab, position, Quaternion.identity);
        treasure.GetComponent<NetworkObject>().Spawn();*/
    }
}