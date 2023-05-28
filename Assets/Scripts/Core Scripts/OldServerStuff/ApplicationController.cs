using System.Threading.Tasks;
using Matchplay.Client;
using Matchplay.Server;
using Matchplay.Shared;
using UnityEngine;
using UnityEngine.Rendering;

public class ApplicationController : MonoBehaviour
{
    public static bool IsServer;

    //Manager instances to be instantiated.
    [SerializeField] private ServerSingleton m_ServerPrefab;

    [SerializeField] private ClientSingleton m_ClientPrefab;

    private ApplicationData m_AppData;

    private async void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);

        //We use EditorApplicationController for Editor launching.
        if (Application.isEditor)
            return;

        //If this is a build and we are headless, we are a server
        await LaunchInMode(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
    }

    public void OnParrelSyncStarted(bool isServer, string cloneName)
    {
#pragma warning disable 4014
        LaunchInMode(isServer, cloneName);
#pragma warning restore 4014
    }

    /// <summary>
    ///     Main project launcher, launched in Start() for builds, and via the EditorApplicationController in-editor
    /// </summary>
    private async Task LaunchInMode(bool isServer, string profileName = "default")
    {
        //init the command parser, get launch args
        m_AppData = new ApplicationData();
        IsServer = isServer;
        if (isServer)
        {
            var serverSingleton = Instantiate(m_ServerPrefab);
            await serverSingleton.CreateServer(); //run the init instead of relying on start.

            var defaultGameInfo = new GameInfo
            {
                gameMode = GameMode.Meditating,
                map = Map.Space,
                gameQueue = GameQueue.Casual
            };

            await serverSingleton.Manager.StartGameServerAsync(defaultGameInfo);
        }
        else
        {
            var clientSingleton = Instantiate(m_ClientPrefab);
            clientSingleton.CreateClient(profileName);

            //We want to load the main menu while the client is still initializing.
            clientSingleton.Manager.ToMainMenu();
        }
    }
}