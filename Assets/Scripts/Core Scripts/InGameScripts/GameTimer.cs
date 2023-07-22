using Matchplay.Server;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    public TMP_Text text;
    private readonly NetworkVariable<float> gameTimer = new(900f);
    public static float staticTimer;

    // Update is called once per frame
    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        if (!GameMaster.gameStartStatic)
        {
            return;
        }
        gameTimer.Value -= Time.deltaTime;
        staticTimer = gameTimer.Value;
        if (gameTimer.Value <= 0) GameTimeOver();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        gameTimer.OnValueChanged += (value, newValue) => { text.SetText(gameTimer.Value.ToString("0.00")); };
    }

    private void GameTimeOver()
    {
        Debug.Log("Game time is up, server shutting down, all players should be killed");
        ServerSingleton.Instance.Manager.CloseServer();
    }

    public static float GetGameTimer()
    {
        return staticTimer;
    }
}