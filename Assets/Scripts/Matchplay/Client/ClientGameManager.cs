using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matchplay.Client;
using Matchplay.Server;
using Matchplay.Shared;
using Matchplay.Shared.Tools;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player = Unity.Services.Matchmaker.Models.Player;



    /// <summary>
    /// Connecting manager of all the components that make a client work
    /// </summary>
    public class ClientGameManager : IDisposable
    {
        public event Action<Matchplayer> MatchPlayerSpawned;
        public event Action<Matchplayer> MatchPlayerDespawned;
        public MatchplayUser User { get; private set; }
        public MatchplayNetworkClient NetworkClient { get; private set; }
        public MatchplayMatchmaker Matchmaker { get; private set; }
        public bool Initialized { get; private set; } = false;

        public string ProfileName { get; private set; }

        public ClientGameManager(string profileName = "default")
        {
            User = new MatchplayUser();
            //Debug.Log($"Beginning with new Profile:{profileName}");
            ProfileName = profileName;
            
            //We can load the mainMenu while the client initializes
#pragma warning disable 4014

            //Disabled warning because we want to fire and forget.
            InitAsync();
#pragma warning restore 4014
        }

        /// <summary>
        /// We do service initialization in parallel to starting the main menu scene
        /// </summary>
        async Task InitAsync()
        {
            var unityAuthenticationInitOptions = new InitializationOptions();
            unityAuthenticationInitOptions.SetProfile($"{ProfileName}{LocalProfileTool.LocalProfileSuffix}");
            await UnityServices.InitializeAsync(unityAuthenticationInitOptions);

            NetworkClient = new MatchplayNetworkClient();
            Matchmaker = new MatchplayMatchmaker();
            var authenticationResult = await AuthenticationWrapper.DoAuth();

            //Catch for if the authentication fails, we can still do local server Testing
            if (authenticationResult == AuthState.Authenticated)
                User.AuthId = AuthenticationWrapper.PlayerID();
            else
                User.AuthId = Guid.NewGuid().ToString();
            //Debug.Log($"did Auth?{authenticationResult} {User.AuthId}");
            Initialized = true;
        }

        public void BeginConnection(string ip, int port)
        {
            Debug.Log($"Starting networkClient @ {ip}:{port}\nWith : {User}");
            NetworkClient.StartClient(ip, port);
        }

        public void Disconnect()
        {
            NetworkClient.DisconnectClient();
        }

       
        

        public async Task CancelMatchmaking()
        {
            await Matchmaker.CancelMatchmaking();
        }

        public void ToMainMenu()
        {
            SceneManager.LoadScene("mainMenu", LoadSceneMode.Single);
        }

        public void AddMatchPlayer(Matchplayer player)
        {
            MatchPlayerSpawned?.Invoke(player);
        }

        public void RemoveMatchPlayer(Matchplayer player)
        {
            MatchPlayerDespawned?.Invoke(player);
        }

        public void SetGameMode(GameMode gameMode)
        {
            User.GameModePreferences = gameMode;
        }

        public void SetGameMap(Map map)
        {
            User.MapPreferences = map;
        }

        public void SetGameQueue(GameQueue queue)
        {
            User.QueuePreference = queue;
        }

        public async Task<MatchmakerPollingResult> GetMatchAsync()
        {
            Debug.Log($"Beginning Matchmaking with {User}");
            var matchmakingResult = await Matchmaker.Matchmake(User.Data);

            if (matchmakingResult.result == MatchmakerPollingResult.Success)
                BeginConnection(matchmakingResult.ip, matchmakingResult.port);
            

            else
                Debug.LogWarning($"{matchmakingResult.result} : {matchmakingResult.resultMessage}");

            return matchmakingResult.result;
        }
        public async Task<MatchmakerPollingResult> GetMatchAsync(Lobby lobby)
        {
            Debug.Log($"Beginning Matchmaking with {User}");
            var matchmakingResult = await Matchmaker.Matchmake(lobby);

            if (matchmakingResult.result == MatchmakerPollingResult.Success)
            {
                BeginConnection(matchmakingResult.ip, matchmakingResult.port);
                await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        
                        {
                            "Ip", new DataObject(DataObject.VisibilityOptions.Member,matchmakingResult.ip)
                        },
                        {
                            "Port", new DataObject(DataObject.VisibilityOptions.Member,matchmakingResult.port.ToString())
                        } 
                    }
                    
                });
            }
               
            else
                Debug.LogWarning($"{matchmakingResult.result} : {matchmakingResult.resultMessage}");

            return matchmakingResult.result;
        }

        public void Dispose()
        {
            NetworkClient?.Dispose();
            Matchmaker?.Dispose();
        }

        public void ExitGame()
        {
            Dispose();
            Application.Quit();
        }

        
    }
