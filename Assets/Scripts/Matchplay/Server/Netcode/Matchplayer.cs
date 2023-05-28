using Matchplay.Client;
using Matchplay.Networking;
using Matchplay.Shared;
using Matchplay.Shared.Tools;
using Unity.Netcode;
using UnityEngine;

namespace Matchplay.Server
{
    /// <summary>
    /// Currently there is no control for moving the player around, only the server does.
    /// The NetworkManager spawns this in automatically, as it is on the designated player object.
    /// </summary>
    public class Matchplayer : NetworkBehaviour
    {
        [HideInInspector]
        public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>();
        [HideInInspector]
        public NetworkVariable<NetworkString> PlayerName = new NetworkVariable<NetworkString>();

        [SerializeField]
        //RendererColorer m_ColorSwitcher;
        public UserData userData;

        public override void OnNetworkSpawn()
        {
            if (IsServer && !IsHost)
                return;

            ClientSingleton.Instance.Manager.AddMatchPlayer(this);
        }

    

        public override void OnNetworkDespawn()
        {
            if (IsServer && !IsHost)
                return;
            if (ApplicationData.IsServerUnitTest)
                return;

            ClientSingleton.Instance.Manager.RemoveMatchPlayer(this);
        }
    }
}