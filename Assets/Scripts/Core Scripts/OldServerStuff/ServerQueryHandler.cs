using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Multiplay;

/// <summary>
///     An example of how to use SQP from the server using the Multiplay SDK.
///     The ServerQueryHandler will report the given information to the Multiplay Service.
/// </summary>
public class ServerQueryHandler : NetworkBehaviour
{
    private const ushort k_DefaultMaxPlayers = 10;
    private const string k_DefaultServerName = "MyServerExample";
    private const string k_DefaultGameType = "MyGameType";
    private const string k_DefaultBuildId = "MyBuildId";
    private const string k_DefaultMap = "MyMap";

    public ushort currentPlayers;

    private IServerQueryHandler m_ServerQueryHandler;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        m_ServerQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(k_DefaultMaxPlayers,
            k_DefaultServerName, k_DefaultGameType, k_DefaultBuildId, k_DefaultMap);
    }

    private async void Start()
    {
        m_ServerQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(k_DefaultMaxPlayers,
            k_DefaultServerName, k_DefaultGameType, k_DefaultBuildId, k_DefaultMap);
    }

    private async void Update()
    {
        m_ServerQueryHandler.UpdateServerCheck();
    }

    public void ChangeQueryResponseValues(ushort maxPlayers, string serverName, string gameType, string buildId)
    {
        m_ServerQueryHandler.MaxPlayers = maxPlayers;
        m_ServerQueryHandler.ServerName = serverName;
        m_ServerQueryHandler.GameType = gameType;
        m_ServerQueryHandler.BuildId = buildId;
    }

    public void PlayerCountChanged(ushort newPlayerCount)
    {
        m_ServerQueryHandler.CurrentPlayers = newPlayerCount;
    }
}