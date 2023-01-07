using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class ServerListContent : MonoBehaviour
{
    public GameObject serverPrefab;

    public void CreateUILobby(Lobby lobby, LobbyManager l)
    {
        var prefab = Instantiate(serverPrefab, transform);
        prefab.GetComponent<ServerPrefabScript>().Init(lobby, l);
    }

    public void ClearLobbies()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
