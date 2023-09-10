using System;
using System.Collections;
using System.Collections.Generic;
using Matchplay.Server;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameMaster : NetworkBehaviour
{
    public  NetworkVariable<bool> gameStart = new NetworkVariable<bool>();
    public static bool gameStartStatic = false;
    public DeathFogManager deathFogManager;
    private int playerCounter;
    private NetworkVariable<float> startTimer = new NetworkVariable<float>(12f);
    public TMP_Text timer;
    
    public override void OnNetworkSpawn()
    {
        
        if (NetworkManager.Singleton.IsServer)
        {
            ServerSingleton.Instance.Manager.NetworkServer.OnServerPlayerSpawned += PlayerCheck;
        }

        gameStart.OnValueChanged += GameStartChange;
        startTimer.OnValueChanged += FloatTimerChange;
    }

    private void GameStartChange(bool previousvalue, bool newvalue)
    {
        gameStart.Value = newvalue;
        gameStartStatic = newvalue;
        Debug.Log("Game start value has changed");
    }

    private void FloatTimerChange(float previous, float newValue)
    {
        //Debug.Log(newValue);
        timer.SetText("Game Starts in " + (int)startTimer.Value);
        startTimer.Value = newValue;
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
            startTimer.Value -= Time.deltaTime;
            
        }

        if (startTimer.Value < 0 && gameStart.Value== false)
        {
            
            //--------- start the game
            DisablePregameTimerClientRpc();
            deathFogManager.StartFog();
            gameStart.Value = true;
        }
    }
    
[ClientRpc]
    private void DisablePregameTimerClientRpc()
    {
        timer.gameObject.SetActive(false);
    }
}
