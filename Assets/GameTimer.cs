using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Matchplay.Server;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : NetworkBehaviour
{
    private NetworkVariable<float> gameTimer = new NetworkVariable<float>(30000f);
    public TMP_Text text;

    public override void OnNetworkSpawn()
    {
        
        base.OnNetworkSpawn();
        gameTimer.OnValueChanged += (value, newValue) =>
        {
            text.SetText(gameTimer.Value.ToString("0.00"));
        };
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!IsServer) return;
        gameTimer.Value -= Time.deltaTime;
        if (gameTimer.Value <= 0)
        {
            GameTimeOver();
        }


    }

    void GameTimeOver()
    {
        Debug.Log("Game time is up, server shutting down, all players should be killed");
        ServerSingleton.Instance.Manager.CloseServer();
    }
}
