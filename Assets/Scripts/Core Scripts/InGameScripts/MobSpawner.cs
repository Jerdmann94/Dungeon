using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MobSpawner : NetworkBehaviour
{
    public ListContainer listContainer;
    public GameObject correspondingTile;

    public GameObject enemyPrefab;

    public List<EnemyData> barracksEnemies;
    public List<EnemyData> bathEnemies;
    public List<EnemyData> beastEnemies;
    public List<EnemyData> excavationEnemies;
    public List<EnemyData> forgeEnemies;
    public List<EnemyData> jailEnemies;
    public List<EnemyData> kitchenEnemies;
    public List<EnemyData> larderEnemies;
    public List<EnemyData> libraryEnemies;
    public List<EnemyData> mushroomEnemies;
    public List<EnemyData> vaultEnemies;
    public List<EnemyData> universalEnemies;

    public void Spawn()
    {
        EnemyData ed = null;
        //Debug.Log(correspondingTile.name);
        switch (correspondingTile.name)
        {
            case var _ when correspondingTile.name.Contains("Barracks"):
                ed = barracksEnemies[Random.Range(0, barracksEnemies.Count)];
                Debug.Log("barracks");
                break;

            case var _ when correspondingTile.name.Contains("BathTileVolume"):
                ed = bathEnemies[Random.Range(0, bathEnemies.Count)];
                Debug.Log("bath");
                break;

            case var _ when correspondingTile.name.Contains("Beast 2 Vol"):
                ed = beastEnemies[Random.Range(0, beastEnemies.Count)];
                Debug.Log("beast");
                break;

            case var _ when correspondingTile.name.Contains("Excavation"):
                ed = excavationEnemies[Random.Range(0, excavationEnemies.Count)];
                Debug.Log("excavation");
                break;

            case var _ when correspondingTile.name.Contains("Forge"):
                ed = forgeEnemies[Random.Range(0, forgeEnemies.Count)];
                Debug.Log("forge");
                break;

            case var _ when correspondingTile.name.Contains("Jail"):
                ed = jailEnemies[Random.Range(0, jailEnemies.Count)];
                break;

            case var _ when correspondingTile.name.Contains("Kitchen"):
                ed = kitchenEnemies[Random.Range(0, kitchenEnemies.Count)];
                break;

            case var _ when correspondingTile.name.Contains("Larder"):
                ed = larderEnemies[Random.Range(0, larderEnemies.Count)];
                break;

            case var _ when correspondingTile.name.Contains("Library"):
                ed = libraryEnemies[Random.Range(0, libraryEnemies.Count)];
                break;

            case var _ when correspondingTile.name.Contains("Mush"):
                ed = mushroomEnemies[Random.Range(0, mushroomEnemies.Count)];
                break;

            case var _ when correspondingTile.name.Contains("Vault"):
                ed = vaultEnemies[Random.Range(0, vaultEnemies.Count)];
                break;
            case var _ when correspondingTile.name.Contains("Wall"):
                Debug.Log("enemy tried to spawn in wall, destroying enemy");
                Destroy(this.gameObject);
                return;
            default:
                ed = universalEnemies[Random.Range(0, universalEnemies.Count)];
                break;
        }

//        Debug.Log(correspondingTile.name);
        var e = Instantiate(enemyPrefab, transform.position, quaternion.identity);
        e.GetComponent<NetworkObject>().Spawn();
        e.transform.SetParent(transform);
        var eController = e.GetComponent<EnemyBrain>();
        eController.SetUpEnemyData(ed);
        var newEnemyId = Guid.NewGuid().ToString();
        eController.id = newEnemyId;
        listContainer.enemyLookUp.Add(newEnemyId, eController);
    }
}