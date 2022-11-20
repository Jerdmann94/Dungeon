using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Multiplay;
using UnityEngine;

public class ServerConfig : MonoBehaviour
{
    
    /// <summary>
    /// A simple example of accessing each of the server config's fields and printing them to the debug console.
    /// </summary>
    public static void LogServerConfig()
    {
        var serverConfig = MultiplayService.Instance.ServerConfig;
        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        Debug.Log($"Port[{serverConfig.Port}]");
        Debug.Log($"QueryPort[{serverConfig.QueryPort}");
        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");
        
    }
    private async void ReadyingServer()
    {
// Once the server is back to a blank slate and ready to accept new players
        await MultiplayService.Instance.ReadyServerForPlayersAsync();
    }

    public void Awake()
    {
        ReadyingServer();
        LogServerConfig();
        
    }
}