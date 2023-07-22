using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PerlinNoiseGen : NetworkBehaviour
{
    public GameObject mobSpawner;
    public GameObject phase2Gen;
    public AstarPath a;
    public List<GameObject> spawners;
    private List<GameObject> phase2NonWalls;
    public float width;
    public float height; 

    public async void InstantiateMobSpawners()
    {
        await Waiting();
        phase2NonWalls = new List<GameObject>();
        foreach (Transform child in phase2Gen.transform)
        {
            if (child.name.Contains("Wall")) continue;
            //Debug.Log(child.name);
            phase2NonWalls.Add(child.gameObject);
        }

        float x = width;
        float y = height;

        float startingPerlingNoiseX = Random.Range(-200000, 200000);
        float startingPerlingNoiseY = Random.Range(-200000, 200000);
        Debug.Log(transform.childCount);
        foreach (Transform o in transform)
        {
#if UNITY_EDITOR
            DestroyImmediate(o.gameObject, false);
#endif
#if !UNITY_EDITOR
          Destroy(o.gameObject);
#endif
        }

        spawners.Clear();
        for (var j = 0; j < x; j++)
        for (var k = 0; k < y; k++)
        {
            var perlinInputX = startingPerlingNoiseX + (.5f + j);
            var perlinInputY = startingPerlingNoiseY + (.5f + k);
            if (Mathf.PerlinNoise(perlinInputX, perlinInputY) < .95) continue;
            //var nameCheck = ("(" + j + ", " + k + ", 0)");
            // Debug.Log(nameCheck + phase2NonWalls[j].name);
            var pos = new Vector3Int((int)(j - x / 2 + 1), (int)(k - y / 2 + 1), 0);
            //Debug.Log(phase2NonWalls[k].transform.position + " " + pos);
            var tile = phase2NonWalls.Find(g => g.transform.position == pos);
            if (!tile) continue;
            var ms = Instantiate(mobSpawner, pos, quaternion.identity);
            ms.transform.SetParent(gameObject.transform);
            ms.GetComponent<MobSpawner>().correspondingTile = tile;
            Debug.Log(tile);
            spawners.Add(ms);

            //Debug.Log("Making mob spawner");
        }

        //SpawnEnemies();
        Debug.Log("done with mob spawning");
    }

    private void Start()
    {
        SpawnEnemies(); // THIS SHOULD BE MOVED OVER TO SERVER ONLY AFTER TESTING MOVEMENT
       
    }

    //THIS SHOULD BE CALLED BY SERVER WHEN GAME STARTS
    private void SpawnEnemies()
    {
//        Debug.Log(IsServer);
  //      Debug.Log("Going into spawners");
        if (IsServer)
        {
            foreach (var spawner in spawners) spawner.GetComponent<MobSpawner>().Spawn();

        }
    //    Debug.Log("done with mob spawning");
    }

    private async Task Waiting()
    {
        await Task.Delay(5000);
    }

    public async void Scan()
    {
        await Waiting();
        a.Scan();
    }
}