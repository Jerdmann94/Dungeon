using System;
using Matchplay.Client;
using Matchplay.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviour
{
    public LobbyManager lobbyManager;
    public TabMaster tabMaster;
    public GameObject ticketUI;
    public TMP_Text timerUI;
    public float timer;
    

    //MATCHMAKING STUFF
    [SerializeField] private ClientSingleton m_ClientPrefab;
    //public event Action<Matchplayer> MatchPlayerSpawned;
    // public event Action<Matchplayer> MatchPlayerDespawned;
    /*public MatchplayUser User { get; private set; }
    public MatchplayNetworkClient NetworkClient { get; private set; }
    public MatchplayMatchmaker Matchmaker { get; private set; }
    public bool Initialized { get; private set; } = false;*/

    // public string ProfileName { get; private set; }

    private ClientGameManager gameManager;

    // LOBBY STUFF
    private AuthState m_AuthState;
    private string m_LocalIP;

    //ClientGameManager gameManager;
    private bool m_LocalLaunchMode;
    private string m_LocalName;
    private string m_LocalPort;


    private async void Start()
    {
        
        Debug.Log("Start of Scene ");
        //SetUpMatchMaker();
        var clientSingleton = Instantiate(m_ClientPrefab);
        clientSingleton.CreateClient("Default");
        SetUpInitialState();
        lobbyManager.InitLobby();
        ticketUI.SetActive(false);
        //Default mode is Matchmaker
        //SetMatchmakerMode();

        m_AuthState = await AuthenticationWrapper.Authenticating();
        
        ClientSingleton.Instance.Manager.NetworkClient.m_NetworkManager.OnClientDisconnectCallback +=
            clientDisconnected;
        
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
        
        lobbyManager.AddItemsToUserData();
        if (m_LocalLaunchMode)
        {
            if (int.TryParse(m_LocalPort, out var localIntPort))
                gameManager.BeginConnection(m_LocalIP, localIntPort);
            else
                Debug.LogError("No valid port in Port Field");
        }
        else
        {
            ticketUI.SetActive(true);
            if (gameManager.Matchmaker.IsMatchmaking)
            {
                Debug.LogWarning("Already matchmaking, please wait or cancel.");
                return;
            }

            var matchResult = await gameManager.GetMatchAsync(lobbyManager.hostLobby);
            switch (matchResult)
            {
                case MatchmakerPollingResult.Success:
                    break;
                case MatchmakerPollingResult.TicketCreationError:
                    CancelPressed();
                    break;
                case MatchmakerPollingResult.TicketCancellationError:
                    CancelPressed();
                    break;
                case MatchmakerPollingResult.TicketRetrievalError:
                    CancelPressed();
                    break;
                case MatchmakerPollingResult.MatchAssignmentError:
                    CancelPressed();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Debug.Log(matchResult);
        }
    }

    public void CancelPressed()
    {
        gameManager.CancelMatchmaking();
        ticketUI.SetActive(false);
    }

    public void clientDisconnected(ulong clientId)
    {
        Debug.Log("Client disconnect from matchmaking manager");
        ticketUI.SetActive(false);
        tabMaster.PressedPlay();
    }

    private void Update()
    {
        if (ticketUI.activeSelf)
        {
            timer += Time.deltaTime;
        }
        else
            timer = 0;

        timerUI.SetText(((int)timer).ToString());

    }
}