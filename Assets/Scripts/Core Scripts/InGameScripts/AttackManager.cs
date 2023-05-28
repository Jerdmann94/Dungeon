using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class AttackManager : NetworkBehaviour

{
    public GameObject attackObjectPrefab;
    public StaticReference managerReference;

    private void Start()
    {
        managerReference.target = gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnAttackItemServerRpc(Vector3 worldPosition, string runeName, ServerRpcParams rpcParams = default)
    {
        //Debug.Log("Doing Attack");
        var clientId = rpcParams.Receive.SenderClientId;
        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        //Check if player has this rune and enough of them to cast this
        var pc = client.PlayerObject.GetComponent<PlayerController>();
        var gameItem = pc.attackPanelManager.runesInAttackPanel.Find(item => item.name == runeName);

        if (gameItem == null)
            return;

        //Debug.Log("Player has rune in inventory");
        var cPos = client.PlayerObject.transform.position;

        // CHECK IF LOCATION IS CLOSE ENOUGH TO PLAYER
        var distance = worldPosition - cPos;
        var direction = distance / distance.magnitude;

        //Debug.Log("raycasting");
        var distF = Vector3.Distance(new Vector3(worldPosition.x, worldPosition.y, 0), cPos);
        var hit = Physics2D.Raycast(cPos, direction, distF, 1);
        if (hit)
            //Debug.Log(hit.collider.gameObject);
            return;
        //Debug.Log("we hit not a wall");
        // SPAWN AN ATTACK OBJECT

        var spawnLoc = new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y), 1);
        var attackObject = Instantiate(attackObjectPrefab, spawnLoc,
            quaternion.identity);
        attackObject.GetComponent<NetworkObject>().Spawn();
    }
}