using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class SpawnTeleporter : NetworkBehaviour
{
    public GameObject teleporter;
    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        var tp = Instantiate(teleporter, transform.position, quaternion.identity);
        tp.GetComponent<NetworkObject>().Spawn();
    }
}

    

