using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Networking.Types;

public class CurrentLobbyManager : MonoBehaviour
{
    public GameObject currentLobbyPlayerPrefab;
    public GameObject playerCollectionPanel;
    public int panelCount;
    public async Task SpawnPlayerPrefabs(Lobby l,LobbyManager lob)
    {

        panelCount = 0;
        ClearPanel();
        foreach (var vPlayer in l.Players)
        {
            var pPanel = Instantiate(currentLobbyPlayerPrefab, playerCollectionPanel.transform);
            var pps = pPanel.GetComponent<PlayerPanelScript>();
            pps.SetNameAndText(
                vPlayer.Data["PlayerName"].Value,
                "0",
                vPlayer.Id,
                lob);
            panelCount++;
            //Debug.Log("HOST ID "+l.HostId + " AUTH ID " + AuthenticationService.Instance.PlayerId);
            if (vPlayer.Id == l.HostId || l.HostId != AuthenticationService.Instance.PlayerId)
            {
                pps.DisableKickForHost();
            } 
            
        }

        await Task.CompletedTask;
    }

    private void ClearPanel()
    {
        foreach (Transform t in playerCollectionPanel.transform)
        {
            Destroy(t.gameObject);
        }
    }
}
