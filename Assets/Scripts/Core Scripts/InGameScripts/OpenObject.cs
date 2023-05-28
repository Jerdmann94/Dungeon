using Unity.Netcode;
using UnityEngine;

public class OpenObject : NetworkBehaviour
{
    public RectTransform uiElement;
    public TreasureScript treasureScript;
    public TreasureInventoryManager treasureInventoryManager;
    public ListContainer listContainer;
    public SpriteRenderer image;

    private void Start()
    {
        uiElement = GameObject.FindWithTag("TreasurePanel").GetComponent<RectTransform>();
        treasureInventoryManager = uiElement.GetComponent<TreasureInventoryManager>();
    }

    private void OnMouseExit()
    {
        image.color = Color.white;
    }

    public void OnMouseOver()
    {
        image.color = Color.red;
        if (Input.GetMouseButtonDown(1))
            CheckIfICanOpenThisChestServerRpc(treasureScript.id);
        //listContainer.GiveTreasureScript(treasureScript);
        //uiElement.gameObject.SetActive(true);
        //treasureInventoryManager.FillUI(treasureScript.GiveItemsForUI());
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckIfICanOpenThisChestServerRpc(string chestID, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        if (PositionCheckUtility.PosCheck(client.PlayerObject.transform.position,
                gameObject.transform.position,
                1.1f))
        {
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { client.ClientId }
                }
            };
            var ts = listContainer.FindTreasure(chestID);
            client.PlayerObject.GetComponent<PlayerController>().inventoryManager.listContainer.lastTreasureScript =
                ts;
            Debug.Log(ts);
            PrepTreasureUIClientRpc(clientRpcParams);
            treasureInventoryManager.ActivateTreasureUIClientRpc(clientRpcParams);
        }
    }

    public void CloseUI()
    {
        uiElement.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void PrepTreasureUIClientRpc(ClientRpcParams clientRpcParams)
    {
        listContainer.GiveTreasureScript(treasureScript);
        //uiElement.gameObject.SetActive(true);
        treasureInventoryManager.FillUI(treasureScript.GiveItemsForUI());
    }
}