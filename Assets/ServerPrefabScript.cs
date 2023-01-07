using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ServerPrefabScript : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text lobbyCode;
    private LobbyManager lobbyManager;
    private Lobby lobby;
    public void Init(Lobby lob, LobbyManager l)
    {
        lobby = lob;
        text.SetText( lobby.Players[0].Data["PlayerNamePublic"].Value);
        lobbyCode.SetText("lobbyId = " + lobby.Id);
        lobbyManager = l;
    }

    public void JoinLobby()
    {
        
        lobbyManager.JoinLobbyByCode(lobby.Id);
    }
}
