using System.Collections;
using System.Collections.Generic;
using Unity.Services.Multiplay;
using UnityEngine;

public class ServerInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }
    private async void Example_ReadyingServer()
    {
// Once the server is back to a blank slate and ready to accept new players
        await MultiplayService.Instance.ReadyServerForPlayersAsync();
    }
   
}
