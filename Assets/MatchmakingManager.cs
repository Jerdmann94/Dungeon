using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matchplay.Client;
using Matchplay.Networking;
using Matchplay.Server;
using Matchplay.Shared;
using Matchplay.Shared.Tools;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;


public class MatchmakingManager : MonoBehaviour
{
    // LOBBY STUFF
    AuthState m_AuthState;

    //ClientGameManager gameManager;
    bool m_LocalLaunchMode;
    string m_LocalIP;
    string m_LocalPort;
    string m_LocalName;

    public LobbyManager lobbyManager;



    //MATCHMAKING STUFF
    [SerializeField] ClientSingleton m_ClientPrefab;
    //public event Action<Matchplayer> MatchPlayerSpawned;
    // public event Action<Matchplayer> MatchPlayerDespawned;
    /*public MatchplayUser User { get; private set; }
    public MatchplayNetworkClient NetworkClient { get; private set; }
    public MatchplayMatchmaker Matchmaker { get; private set; }
    public bool Initialized { get; private set; } = false;*/

    // public string ProfileName { get; private set; }

    private ClientGameManager gameManager;


    async void Start()
    {
        //SetUpMatchMaker();
        var clientSingleton = Instantiate(m_ClientPrefab);
        clientSingleton.CreateClient("Default");
        SetUpInitialState();
        lobbyManager.InitLobby();
        //Default mode is Matchmaker
        //SetMatchmakerMode();

        m_AuthState = await AuthenticationWrapper.Authenticating();

    }

    /*private async void SetUpMatchMaker()
    {
        User = new MatchplayUser();
        ProfileName = "Default";
        var unityAuthenticationInitOptions = new InitializationOptions();
        unityAuthenticationInitOptions.SetProfile($"{ProfileName}{LocalProfileTool.LocalProfileSuffix}");
        await UnityServices.InitializeAsync(unityAuthenticationInitOptions);
        NetworkClient = new MatchplayNetworkClient();
        Matchmaker = new MatchplayMatchmaker();
        var authenticationResult = await AuthenticationWrapper.DoAuth();
        if (authenticationResult == AuthState.Authenticated)
            User.AuthId = AuthenticationWrapper.PlayerID();
        else
            User.AuthId = Guid.NewGuid().ToString();
        Debug.Log($"did Auth?{authenticationResult} {User.AuthId}");
        Initialized = true;
        
        
    }*/
    private void SetUpInitialState()
    {
        gameManager = ClientSingleton.Instance.Manager;
        //Set the game manager casual gameMode defaults to whatever the UI starts with
        gameManager.SetGameMode(Enum.Parse<GameMode>("Staring"));
        gameManager.SetGameMap(Enum.Parse<Map>("Lab"));
        gameManager.SetGameQueue(Enum.Parse<GameQueue>("Casual"));
        m_LocalPort = "7777";
        m_LocalIP = "127.0.0.1";

    }


    public void SetLocalModeTrue()
    {
        m_LocalLaunchMode = true;
    }
    public void SetLocalModeFalse()
    {
        m_LocalLaunchMode = false;
    }

    public async void PlayButtonPressed()
    {
        Debug.Log(m_LocalLaunchMode);
        if (m_LocalLaunchMode)
        {
            if (int.TryParse(m_LocalPort, out var localIntPort))
            {
                gameManager.BeginConnection(m_LocalIP, localIntPort);

            }
            else
            {
                Debug.LogError("No valid port in Port Field");
            }
        }
        else
        {
            if (gameManager.Matchmaker.IsMatchmaking)
            {
                Debug.LogWarning("Already matchmaking, please wait or cancel.");
                return;
            }

            var matchResult = await gameManager.GetMatchAsync(lobbyManager.hostLobby);


        }
    }
}


