using System;
using System.Collections;
using System.Collections.Generic;
using Matchplay.Server;
using Unity.Netcode;
using UnityEngine;

public class GameMaster : NetworkBehaviour
{
    public  NetworkVariable<bool> gameStart = new NetworkVariable<bool>();
    public static bool gameStartStatic = false;
    public DeathFogManager deathFogManager;
    private int playerCounter;
    private float startTimer = 12f;
    
    public override void OnNetworkSpawn()
    {
        
        if (NetworkManager.Singleton.IsServer)
        {
            ServerSingleton.Instance.Manager.NetworkServer.OnServerPlayerSpawned += PlayerCheck;
        }

        gameStart.OnValueChanged += GameStartChange;
    }

    private void GameStartChange(bool previousvalue, bool newvalue)
    {
        gameStart.Value = newvalue;
        gameStartStatic = newvalue;
        Debug.Log("Game start value has changed");
    }


    private void PlayerCheck(Matchplayer player)
    {
        playerCounter++;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (playerCounter > 0)
        {
            startTimer -= Time.deltaTime;
        }

        if (startTimer < 0 && gameStart.Value== false)
        {
            
            //--------- start the game
            deathFogManager.StartFog();
            gameStart.Value = true;
        }
    }
}
