using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matchplay.Client;
using MyBox;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using Player = Unity.Services.Lobbies.Models.Player;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviour
{
    public string lobbyName;
    public int maxPlayers;
    public ServerListContent serverListContent;
    public ShopSceneManager shopSceneManager;
    public CurrentLobbyManager currentLobbyManager;
    public TabMaster tabMaster;

    public LobbyInventoryManager inventoryManager;
    private float heartBeatTimer;

    public Lobby hostLobby;
    private readonly float maxHeartBeatTimer = 15f;
    private readonly float maxPollTimer = 2f;
    private string playerName;
    private float pollTimer;


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

            if (hostLobby.Data.ContainsKey("Ip") &&
                p.Id != hostLobby.HostId) //CHECKING FOR TICKET ASSIGNMENT IF YOU ARE NOT HOST
            {
                var ticket = await MatchmakerService.Instance.GetTicketAsync(hostLobby.Data["TicketId"].Value);
                if (ticket != null)
                {
                    HandleTicket(ticket);
                }
                var ip = hostLobby.Data["Ip"].Value;
                var port = int.Parse(hostLobby.Data["Port"].Value);
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
                //ClientSingleton.Instance.Manager.BeginConnection(assignment.Ip, (int)assignment.Port);
                break;
            case MultiplayAssignment.StatusOptions.InProgress:
                Debug.Log("ticket in progress");
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
        tabMaster.MakeSureLoadingIsOn();
        // Debug.Log(serverListContent);
        try
        {
            AuthenticationService.Instance.SignedIn += async () =>
            {
                await CreateLobby();
                await ListLobbies();
                await currentLobbyManager.SpawnPlayerPrefabs(hostLobby, this);
                await shopSceneManager.InitShop();
                tabMaster.PressedPlay();
                Debug.Log("pressed play");
            };

           
          
            //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        if (AuthenticationService.Instance.IsSignedIn)
        {
            tabMaster.PressedPlay();
        }
    }

    public async void JoinLobbyByCode(string code)
    {
        try
        {
            var joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            Debug.Log("Join by lobby code = " + code);
            hostLobby = await Lobbies.Instance.JoinLobbyByIdAsync(code, joinLobbyByIdOptions);
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
                    continue;
                Debug.Log("Lobbies Query, Lobby code " + l.LobbyCode +
                          ", lobby name  " + lobbyName);
                serverListContent.CreateUILobby(l, this);
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
                if (hostLobby.Players.Find(player => player.Id == AuthenticationService.Instance.PlayerId) != null)
                {
                    Debug.Log("We are already in a lobby and we should not create a new one," +
                              " hopefully this only happens when we return from a game finished game");
                    return;
                }

            var createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>()
            };
            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
//            Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.LobbyCode + " " + lobby.Data);
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
                }
            }
        };
    }

    public async void RefreshLobbies()
    {
        await ListLobbies();
    }

    public async Task RefreshCurrentLobby()
    {
        await currentLobbyManager.SpawnPlayerPrefabs(hostLobby, this);
    }


    public async void KickPlayer(string playerId)
    {
        await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, playerId);
        AfterLeavingLobby();
    }

    public void AddItemsToUserData()
    {
       //adding items to matchmatcher user data
        Debug.Log(ClientSingleton.Instance.Manager.User.Data.userGamePreferences);
        ClientSingleton.Instance.Manager.User.Data.userGamePreferences.inventoryItems = "";
        ClientSingleton.Instance.Manager.User.Data.userGamePreferences.inventoryItems =
            JsonConvert.SerializeObject(inventoryManager.lootInInventory);
        ClientSingleton.Instance.Manager.User.Data.userGamePreferences.equipment = "";
        ClientSingleton.Instance.Manager.User.Data.userGamePreferences.equipment =
            JsonConvert.SerializeObject(inventoryManager.equipment);
        Debug.Log(ClientSingleton.Instance.Manager.User.Data.userGamePreferences.equipment);
        Debug.Log(ClientSingleton.Instance.Manager.User.Data.userGamePreferences.inventoryItems);
        
        
       // removing items from economy so that they dont have the items if they die
       inventoryManager.lootInInventory.ForEach(item =>
       {
           EconomyService.Instance.PlayerInventory.DeletePlayersInventoryItemAsync(item.id);
       });
       foreach (var keyValuePair in inventoryManager.equipment)
       {
           if (keyValuePair.Value == null)
           {
               continue;
           }
           EconomyService.Instance.PlayerInventory.DeletePlayersInventoryItemAsync(keyValuePair.Value.id);
       }
    }
}