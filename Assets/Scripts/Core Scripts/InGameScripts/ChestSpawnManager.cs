using Unity.Netcode;
using UnityEngine;

public class ChestSpawnManager : NetworkBehaviour
{
    public GameObject treasurePrefab;
    public ListContainer listContainer;

    public override void OnNetworkSpawn()
    {
        listContainer.Init();
        base.OnNetworkSpawn();
    }

    public void SpawnChest(Vector3 pos, LootTable lootTable)
    {
        if (!IsServer)
            return;
        //Debug.Log("spawning chest at " + pos);
        var treasure = Instantiate(treasurePrefab, pos, Quaternion.identity);
        treasure.GetComponent<NetworkObject>().Spawn();
        treasure.GetComponent<TreasureScript>().FillChest(lootTable);
    }
}