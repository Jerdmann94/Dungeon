using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawnManager : NetworkBehaviour
{
    public List<GameObject> enemySpawns;

    public GameObject enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer)
            return;
        
        foreach (var spawn in enemySpawns)
        {
            var enemy = Instantiate(enemyPrefab, spawn.transform.position, quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();
        }
        Debug.Log("Enemies spawned");
    }

    
}
