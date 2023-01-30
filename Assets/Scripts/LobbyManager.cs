using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Matchplay.Client;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using Player = Unity.Services.Lobbies.Models.Player;
using Random = UnityEngine.Random;


public class LobbyManager : MonoBehaviour
{
    public String lobbyName;
    public int maxPlayers;
    public ServerListContent serverListContent;
    
    public Lobby hostLobby;
    private float heartBeatTimer;
    private float maxHeartBeatTimer = 15f;
    private float pollTimer;
    private float maxPollTimer = 2f;
    private string playerName;
    public CurrentLobbyManager currentLobbyManager;


    private void Update()
    {
        if (hostLobby == null) return;
        
        
        
        DoHeartBeat();
        PollForUpdates();
    }

    private async void PollForUpdates()
    {
        
        pollTimer -= Time.deltaTime;
        if (!(pollTimer < 0)) return;
        pollTimer = maxPollTimer;
        hostLobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
        if (hostLobby.Data != null)
        {
            var p = hostLobby.Players.Find(p => p.Id == AuthenticationService.Instance.PlayerId);
            Debug.Log("this player is checking for tickets in data " + hostLobby.Data.Count);
       
            if (hostLobby.Data.ContainsKey("Ip") && p.Id != hostLobby.HostId)//CHECKING FOR TICKET ASSIGNMENT IF YOU ARE NOT HOST
            {
                /*var ticket = await MatchmakerService.Instance.GetTicketAsync(hostLobby.Data["TicketId"].Value);
                if (ticket != null)
                {
                    HandleTicket(ticket);
                }*/
                var ip = hostLobby.Data["Ip"].Value;
                int port = int.Parse(hostLobby.Data["Port"].Value);
                ClientSingleton.Instance.Manager.BeginConnection(ip, port);
                hostLobby.Data.Remove("Ip");
                hostLobby.Data.Remove("Port");
            }
        }
        if (currentLobbyManager.panelCount == hostLobby.Players.Count) return;
        if (hostLobby.Players.Find(pp => pp.Id == AuthenticationService.Instance.PlayerId) == null)
        {
            Debug.Log("player was not in host server, create new server");
            await CreateLobby();
        }
        await currentLobbyManager.SpawnPlayerPrefabs(hostLobby, this);

    }

    private void HandleTicket(TicketStatusResponse ticket)
    {
        Debug.Log("polling for ticket as non Host");
        var assignment = ticket.Value as MultiplayAssignment;
        switch (assignment.Status)
        {
            case MultiplayAssignment.StatusOptions.Found:
                Debug.Log("Found connection for client that is not host, begin joining ip and port");
                ClientSingleton.Instance.Manager.BeginConnection(assignment.Ip, (int)assignment.Port);
                break;
            case MultiplayAssignment.StatusOptions.InProgress:
                //...
                break;
            case MultiplayAssignment.StatusOptions.Failed:
                
                Debug.LogError("Failed to get ticket status. Error: " + assignment.Message);
                break;
            case MultiplayAssignment.StatusOptions.Timeout:
                
                Debug.LogError("Failed to get ticket status. Ticket timed out.");
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    private async void DoHeartBeat()
    {
        if (hostLobby.HostId != AuthenticationService.Instance.PlayerId) return;
        heartBeatTimer -= Time.deltaTime;
        if (!(heartBeatTimer < 0f)) return;
        heartBeatTimer = maxHeartBeatTimer;
        await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
    }

    public async void InitLobby()
    {
        playerName = "TestName" + Random.Range(1, 99);
       // Debug.Log(serverListContent);
        try
        {
            AuthenticationService.Instance.SignedIn += async () =>
            {
                await CreateLobby();
                await ListLobbies();
                await currentLobbyManager.SpawnPlayerPrefabs(hostLobby,this);
                
            };
            
            //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
       
        
    }

    public async void JoinLobbyByCode(String code)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer()
            };
            Debug.Log("Join by lobby code = " + code); 
            hostLobby = await Lobbies.Instance.JoinLobbyByIdAsync(code,joinLobbyByIdOptions);
            /*foreach (var player in hostLobby.Players)
            {
                Debug.Log(player.Id + " " + AuthenticationService.Instance.PlayerId);
            }*/
            
            
            await currentLobbyManager.SpawnPlayerPrefabs(hostLobby, this);
            await ListLobbies();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public async Task ListLobbies()
    {
        try
        {
            //hostLobby ??= await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            var queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            
            serverListContent.ClearLobbies();
            foreach (var l in queryResponse.Results)
            {
                if (l.HostId == AuthenticationService.Instance.PlayerId ||
                    l.IsPrivate || l.Players.Count == 0)
                {
                    continue;
                }
                Debug.Log("Lobbies Query, Lobby code " +l.LobbyCode +
                          ", lobby name  " + lobbyName);
                serverListContent.CreateUILobby(l,this);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    private async Task CreateLobby()
    {
        try
        {
            if (hostLobby != null)
            {
                if (hostLobby.Players.Find(player => player.Id == AuthenticationService.Instance.PlayerId ) != null)
                {
                    Debug.Log("We are already in a lobby and we should not create a new one," +
                              " hopefully this only happens when we return from a game finished game");
                    return;
                }
            }
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>()
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers,createLobbyOptions);
            hostLobby = lobby;
            Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.LobbyCode + " " + lobby.Data);
            await RefreshCurrentLobby();
        }
        catch (Exception e)
        {
            Debug.Log("Lobby failed to create");
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        
        await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId);
        await CreateLobby();
        AfterLeavingLobby();
    }
    

    private async void AfterLeavingLobby()
    {
        Debug.Log("Player left party");
        
        await RefreshCurrentLobby();
        await ListLobbies();
    
    }

    public Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {
                    "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                },
                {
                    "PlayerNamePublic", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName)
                },
                {
                    "Level", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "0")
                },
                {
                    "userAuthId",
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,
                        ClientSingleton.Instance.Manager.User.AuthId)
                },
            }
        };
    }

    public async void RefreshLobbies()
    {
        await ListLobbies();
    }

    public async Task RefreshCurrentLobby()
    {
        await currentLobbyManager.SpawnPlayerPrefabs(hostLobby,this);
    }


    public async void KickPlayer(string playerId)
    {
        await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, playerId);
        AfterLeavingLobby();
    }
}
