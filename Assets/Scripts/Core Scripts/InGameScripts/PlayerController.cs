using System.Collections.Generic;
using Core_Scripts.EnemyScripts;
using DG.Tweening;
using Matchplay.Client;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    public PlayerSpawnPointList playerSpawnPointList;

    //public NetworkVariable<float>moveCoolDown =new NetworkVariable<float>(0);
    public NetworkVariable<float> digCoolDown = new();

    public BoxCollider2D north;
    public BoxCollider2D south;
    public BoxCollider2D east;
    public BoxCollider2D west;
    public LayerMask wallMask;
    public LayerMask playerMask;
    public LayerMask enemyMask;
    public GameObject destroyedWallObj;
    public Slider healthSlider;

    public PlayerStatBlock statBlock;

    public InventoryManager inventoryManager;
    public AttackPanelManager attackPanelManager;
    public StaticReference textManager;
    //EVENTS

    [SerializeField] private GameEvent closeTreasureUI;
    // PSEUDO INVENTORY LIST FOR NETWORK

    public NetworkList<FixedString512Bytes> equipJson;


    public Camera mainCam;
    public float fogDamageTimer = 2f;
   

    private void Awake()
    {
        equipJson = new NetworkList<FixedString512Bytes>();
        
        if (!IsServer) return;
    }

    private void Update()
    {
        if (!IsServer && IsOwner)
        {
            // -------------  CAMERA STUFF --------------------------
            var position = transform.position;
            if (mainCam == null) mainCam = Camera.main;
            mainCam!.transform.position = new Vector3(position.x, position.y, -10);
        }
        
        if (!GameMaster.gameStartStatic)
        {
            return;
        }
        //-----------------------------SERVER STUFF
        if (IsServer)
        {
            
            statBlock.UpdateMoveCooldown();
            if (digCoolDown.Value > 0) digCoolDown.Value -= Time.deltaTime;
            if (fogDamageTimer > 0) fogDamageTimer -= Time.deltaTime;
            if (!DeathFogManager.CheckFogSafety(transform.position)&& fogDamageTimer <= 0)
            {
                HealthChangeFromServer(-10);
                fogDamageTimer = 2f;
            } 
        }
            
        

        if (!IsOwner)
            return;
        //-----------------------------CHECK FOR DEATH -------------------------
            //DISABLING DEATH CHECK DURING OTHER TESTING

            if (statBlock.CurrentHealth <= 0)
            { 
                Debug.Log("player death, current health less than 0 " + statBlock.CurrentHealth);
                ClientSingleton.Instance.Manager.Disconnect();
                //DeathServerRpc();
            }
            
        
       
        

        // CHECK FOR DESTRUCTION 
        if (Input.GetKey(KeyCode.F) && digCoolDown.Value <= 0)
            DestroyWallAtLocationServerRpc(mainCam.ScreenToWorldPoint(Input.mousePosition));

        // --------------------- INPUT ----------------------------
        var moveX = 0;
        var moveY = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveY += 1;

        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveX -= 1;

        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveX += 1;

        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveY -= 1;

        if (moveX != 0 || moveY != 0) MovePlayerServerRpc(moveX, moveY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsClient)
            return;
     //   Debug.Log(collision.gameObject.name);
//        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Attack") || collision.gameObject.CompareTag("EnemyAttack"))
        {

            var attackScript = collision.gameObject.GetComponent<AttackScript>();
            if (attackScript.hitList.Contains(gameObject)) return;
            attackScript.hitList.Add(gameObject);

            textManager.Target.GetComponent<TextManager>()
                .CreatePopUp(transform.position, attackScript.damage, Color.red);
            HealthChangeServerRpc(-attackScript.damage);
            healthSlider.value = statBlock.CurrentHealth;
        }
    }

    public override void OnNetworkSpawn()
    {
        inventoryManager = GameObject.FindWithTag("InventoryManager").GetComponent<InventoryManager>();
        attackPanelManager = GameObject.FindWithTag("AttackPanelObjectCollection").GetComponent<AttackPanelManager>();


        inventoryManager.pc = this;
        
        
        if (!IsOwner) return;
        //mainCam = Camera.main;

        //MovePlayerToSpawnServerRpc();

        equipJson ??= new NetworkList<FixedString512Bytes>();
        equipJson.OnListChanged += inventoryManager.UpdateInventoryDelegate;
    }


    [ServerRpc]
    private void MovePlayerToSpawnServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        var rand = Random.Range(0, playerSpawnPointList.spawnerUser.Count);

        var spawnerPod = playerSpawnPointList.spawnPods[rand];
        transform.position = spawnerPod.transform.GetChild(0).transform.position;
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { client.ClientId }
            }
        };
        //statBlock.UpdateStats(inventoryManager.equipment);
        //inventoryManager.SendStatsToClientRpc(JsonConvert.SerializeObject(statBlock), clientRpcParams);
    }


    [ServerRpc]
    private void MovePlayerServerRpc(int moveX, int moveY, ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        //Debug.Log(clientId);
        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        var pc = client.PlayerObject.GetComponent<PlayerController>();
        if (pc.statBlock.CheckMoveCoolDown()) return;
        var transform2 = client.PlayerObject.transform.position;
        var position = new Vector3(Mathf.RoundToInt(transform2.x), Mathf.RoundToInt(transform2.y), 0);
        var newPosition = new Vector3();
        var layerMask = wallMask;
        //var layerMask = LayerMask.GetMask(wallMask.ToString(), playerMask.ToString());
        if (moveX != 0)
        {
            if (moveX < 0 && !west.IsTouchingLayers(layerMask) && !west.IsTouchingLayers(playerMask) &&
                !west.IsTouchingLayers(enemyMask))
                newPosition.x = +-1;
            else if (moveX > 0 && !east.IsTouchingLayers(layerMask) && !east.IsTouchingLayers(playerMask) &&
                     !east.IsTouchingLayers(enemyMask)) newPosition.x = +1;
        }
        if (moveY != 0)
        {
            if (moveY < 0 && !south.IsTouchingLayers(layerMask) && !south.IsTouchingLayers(playerMask) &&
                !south.IsTouchingLayers(enemyMask))
                newPosition.y = +-1;
            else if (moveY > 0 && !north.IsTouchingLayers(layerMask) && !north.IsTouchingLayers(playerMask) &&
                     !north.IsTouchingLayers(enemyMask)) newPosition.y = +1;
        }
        position = new Vector3(newPosition.x + position.x, newPosition.y + position.y, 0);
        pc.statBlock.ResetMoveCoolDown();
        client.PlayerObject.transform.DOMove(position, 5 / pc.statBlock.GetSpeedStat());
        //CHECK IF NEW POSITION IS OUTSIDE OF THE REACH OF THE CHEST YOU ARE IN
        if (inventoryManager.listContainer.ReturnTreasureScript() == null) return;
        if (PositionCheckUtility.PosCheck(position,
                pc.inventoryManager.listContainer.lastTreasureScript.gameObject.transform.position,
                1.1f)) return;
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { client.ClientId }
            }
        };

        CloseTreasureClientRpc(clientRpcParams);


        //transform1.position = position;
    }

    [ServerRpc]
    private void DestroyWallAtLocationServerRpc(Vector3 worldPosition, ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;

        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        var cPos = client.PlayerObject.transform.position;

        // CHECK IF LOCATION IS CLOSE ENOUGH TO PLAYER
        var distance = worldPosition - cPos;
        if (Mathf.Abs(distance.x) < 1 || Mathf.Abs(distance.y) < 1)
        {
            var direction = distance / distance.magnitude;
            var hit = Physics2D.Raycast(transform.position, direction, 1f, 1);
            if (hit)
            {
                // REMOVE WALL TILE
                var hitPos = hit.transform.position;
                hit.collider.gameObject.GetComponent<NetworkObject>().Despawn();

                //SPAWN NEW GROUND TILE
                var destroyObj = Instantiate(destroyedWallObj, hitPos, quaternion.identity);
                destroyObj.GetComponent<NetworkObject>().Spawn(true);
                client.PlayerObject.GetComponent<PlayerController>().digCoolDown.Value += 10;
            }
        }

        //PRINT SUCCESS/FAILURE
    }

    [ServerRpc(RequireOwnership = false)]
    private void HealthChangeServerRpc(int i, ServerRpcParams serverRpcParams = default)
    {
        HealthChangeFromServer(i);
        
        /*var clientId = serverRpcParams.Receive.SenderClientId;
        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { client.ClientId }
                }
            };
        HealthChangeClientRpc(i, clientRpcParams);*/
    }

    public void HealthChangeFromServer(int i)
    {
        if (!IsServer)
            return;
        //Debug.Log("Health Change from Server");
        textManager.Target.GetComponent<TextManager>().CreatePopUp(transform.position, i, Color.red);
        //healthSlider.value = statBlock.currentHealth.Value;
        healthSlider.maxValue = statBlock.MaxHealth;
        healthSlider.value = statBlock.CurrentHealth;
        HealthChangeClientRpc(i);
       
    }

    [ClientRpc]
    public void HealthChangeClientRpc(int i,ClientRpcParams clientRpcParams = default)
    {
        Debug.Log(statBlock.CurrentHealth + " current health " +
                  statBlock.MaxHealth + " max health");
        statBlock.CurrentHealth += i;
        Debug.Log(statBlock.CurrentHealth + " current health " +
                  statBlock.MaxHealth + " max health");
        healthSlider.maxValue = statBlock.MaxHealth;
        healthSlider.value = statBlock.CurrentHealth;
    }

    
    [ServerRpc]
    private void DeathServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;


        //if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        MovePlayerToSpawnServerRpc();
        client.PlayerObject.GetComponent<PlayerController>().HealthChangeServerRpc(100);
    }
    //CLIENT SIDE STUFF

    [ClientRpc]
    public void CloseTreasureClientRpc(ClientRpcParams clientRpcParams = default)
    {
        //Debug.Log("emitting close treasure event");
        closeTreasureUI.TriggerEvent();
    }
}