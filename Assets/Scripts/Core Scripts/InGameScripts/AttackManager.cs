using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class AttackManager : NetworkBehaviour

{
    public GameObject attackObjectPrefab;
    public StaticReference managerReference;


    private void Start()
    {
        managerReference.Target = gameObject;
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

//        Debug.Log("Rune was item " + gameItem);
        if (gameItem == null)
            return;

        //Debug.Log("Player has rune in inventory");
        var cPos = client.PlayerObject.transform.position;

        // CHECK IF LOCATION IS CLOSE ENOUGH TO PLAYER
        var distance = worldPosition - cPos;
        var direction = distance / distance.magnitude;


        var distF = Vector3.Distance(new Vector3(worldPosition.x, worldPosition.y, 0), cPos);
        var hit = Physics2D.Raycast(cPos, direction, distF, 1);
        if (hit)
            return;
      
        //Get attack object prefrab from item
        var prefab = gameItem.attackPrefab;
        //ROLL DAMAGE FOR OBJECT
        var damage = RollDamage(pc.statBlock,gameItem.rangeLowDamage,gameItem.rangeHighDamage, gameItem.damageType);
        // SPAWN AN ATTACK OBJECT
        var spawnLoc = new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y), 1);
        var attackObject = Instantiate(prefab, spawnLoc,
            quaternion.identity);

        //CHECK WHAT KIND OF SCRIPT IS ATTATCHED TO OBJECT
        var attackScript = attackObject.GetComponent<AttackScript>();
        if ( attackScript != null)
        {
            attackObject.GetComponent<AttackScript>().damage = damage;
            
        }
        else if (attackObject.GetComponent<AoeParent>() != null)
        {
            attackObject.GetComponent<AoeParent>().SetChildDamage(damage);
        }
        
        attackObject.GetComponent<NetworkObject>().Spawn();
    }

    private int RollDamage(PlayerStatBlock ps, int rangeLow, int rangeHigh, DamageType gameItemDamageType)
    {
        return ps.RollDamage(rangeLow, rangeHigh,gameItemDamageType);
    }
}