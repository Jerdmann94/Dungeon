using System;
using DG.Tweening;
using Matchplay.Client;
using MyBox;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PlayerController : NetworkBehaviour
    {
        public PlayerSpawnPointList playerSpawnPointList;
        public NetworkVariable<float>moveCoolDown =new NetworkVariable<float>(0);
        public NetworkVariable<float>digCoolDown =new NetworkVariable<float>(0);
        public float moveSpeed=.5f;
        public BoxCollider2D north;
        public BoxCollider2D south;
        public BoxCollider2D east;
        public BoxCollider2D west;
        public LayerMask wallMask;
        public LayerMask playerMask;
        public LayerMask enemyMask;
        public GameObject destroyedWallObj;
        public Slider healthSlider;
        public NetworkVariable<int> health = new NetworkVariable<int>(100);

        public InventoryManager inventoryManager;
        public AttackPanelManager attackPanelManager;
        public StaticReference textManager;
        
        private Camera mainCam;
        //EVENTS

        [SerializeField] private GameEvent closeTreasureUI;
        // PSEUDO INVENTORY LIST FOR NETWORK

        public NetworkList<FixedString512Bytes> equipJson;

        private void Awake()
        {
            equipJson = new NetworkList<FixedString512Bytes>();
        }

        public override void OnNetworkSpawn()
        {
            inventoryManager = GameObject.FindWithTag("InventoryManager").GetComponent<InventoryManager>();
            attackPanelManager = GameObject.FindWithTag("AttackPanelObjectCollection").GetComponent<AttackPanelManager>();
            health.OnValueChanged += (value, newValue) =>
            {
                //Debug.Log(value + " new value " + newValue);
                healthSlider.value = newValue;
            };
            if (!IsOwner) return;
            mainCam = Camera.main;
            
            MovePlayerToSpawnServerRpc();

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
        }

        void Update()
        {
            if (IsServer)
            {
               
                foreach (var client in NetworkManager.ConnectedClients.Values)
                {
                    var pc = client.PlayerObject.GetComponent<PlayerController>();
                    if (pc.moveCoolDown.Value > 0)
                    { 
                        pc.moveCoolDown.Value -=  Time.deltaTime;
                    }

                    if (pc.digCoolDown.Value > 0)
                    {
                        pc.digCoolDown.Value -=  Time.deltaTime;
                    }
                }
            }
            if (!IsOwner)
                return;
            //-----------------------------CHECK FOR DEATH -------------------------
            if (health.Value <=0)
            {
                ClientSingleton.Instance.Manager.Disconnect();
                //DeathServerRpc();
            }
            // -------------  CAMERA STUFF --------------------------
            var position = transform.position;
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
            mainCam!.transform.position = new Vector3(position.x, position.y, -10);
            
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

            if (moveX != 0 || moveY != 0)
            {
                MovePlayerServerRpc(moveX,moveY);
            }
            
        }

        

        [ServerRpc]
        private void MovePlayerServerRpc(int moveX,int moveY,ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            
            if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
            var client = NetworkManager.ConnectedClients[clientId];
            var cd = client.PlayerObject.GetComponent<PlayerController>().moveCoolDown;
            if (cd.Value > 0)
            {
                return;
            }
               
            
            
            var transform2 = client.PlayerObject.transform.position;
            var position = new Vector3(Mathf.RoundToInt(transform2.x), Mathf.RoundToInt(transform2.y), 0);
            var newPosition = new Vector3();
            var layerMask = wallMask;
            //var layerMask = LayerMask.GetMask(wallMask.ToString(), playerMask.ToString());
            if (moveX != 0) {
                if (moveX < 0&& !west.IsTouchingLayers(layerMask) && !west.IsTouchingLayers(playerMask) && !west.IsTouchingLayers(enemyMask)  )
                {
                    newPosition.x =+ -1;
                }
                else if (moveX > 0&& !east.IsTouchingLayers(layerMask) && !east.IsTouchingLayers(playerMask) && !east.IsTouchingLayers(enemyMask) )
                {
                    newPosition.x =+ 1;
                }
            }
            if (moveY != 0) {
                if (moveY < 0 && !south.IsTouchingLayers(layerMask) && !south.IsTouchingLayers(playerMask)&& !south.IsTouchingLayers(enemyMask) )
                {
                    newPosition.y =+ -1;
                }
                else if (moveY > 0 &&!north.IsTouchingLayers(layerMask) && !north.IsTouchingLayers(playerMask)&& !north.IsTouchingLayers(enemyMask) )
                {
                    newPosition.y =+ 1;
                }
            }
            /*Debug.Log(!north.IsTouchingLayers(layerMask)
                      + " s = " + !south.IsTouchingLayers(layerMask)
                      + " e = " + !east.IsTouchingLayers(layerMask) 
                      + " w = " + !west.IsTouchingLayers(layerMask) );*/
            
            position = new Vector3(newPosition.x + position.x, newPosition.y+
                                                               position.y, 0);
            var pc = client.PlayerObject.GetComponent<PlayerController>();
            pc.moveCoolDown.Value+=moveSpeed;
            client.PlayerObject.transform.DOMove(position, moveSpeed*.5f);
            //CHECK IF NEW POSITION IS OUTSIDE OF THE REACH OF THE CHEST YOU ARE IN
            if (inventoryManager.listContainer.ReturnTreasureScript() == null)
            {
                Debug.Log("no treasure script in list container");
                return;
            }
            if (PositionCheckUtility.PosCheck(position,
                    pc.inventoryManager.listContainer.lastTreasureScript.gameObject.transform.position,
                    1.1f)) return;
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]{client.ClientId}
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
                var direction = distance/distance.magnitude;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, 1);
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
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsClient)
                return;
            if (!collision.gameObject.CompareTag("Attack"))
                return;
            
            var attackScript = collision.gameObject.GetComponent<AttackScript>();
            if (attackScript.hitList.Contains(gameObject))
            {
               
                return;
            }
            attackScript.hitList.Add(gameObject);
            
            textManager.target.GetComponent<TextManager>().CreatePopUp(transform.position, attackScript.damage, Color.red);
            HealthChangeServerRpc(-attackScript.damage);
            healthSlider.value = health.Value;
        }
        [ServerRpc(RequireOwnership = false)]
        private void HealthChangeServerRpc(int i)
        {
            HealthChangeFromServer(i);
        }

        public void HealthChangeFromServer(int i)
        {
            if (!IsServer)
                return;
            Debug.Log("Health Change from Server");
            textManager.target.GetComponent<TextManager>().CreatePopUp(transform.position, i, Color.red);

            health.Value += i;
            healthSlider.value = health.Value;
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
        public void SendItemClientRpc(string itemJson,ClientRpcParams clientRpcParams = default)
        {
            GameItem item = JsonUtility.FromJson<GameItem>(itemJson);
            Debug.Log(item);
        }
        [ClientRpc]
        public void CloseTreasureClientRpc(ClientRpcParams clientRpcParams = default)
        {
            Debug.Log("emitting close treasure event");
            closeTreasureUI.TriggerEvent();
        }
    }
