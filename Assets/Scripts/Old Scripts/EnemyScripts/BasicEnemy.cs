
using Pathfinding;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemy : NetworkBehaviour
{
    public Slider healthSlider;
    public int maxHealth;
    
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    [SerializeField] private AIDestinationSetter setter;
    private BasicEnemyMelee basicEnemyMelee;

    public StaticReference textManager;

    public GameTreasureSpawnEvent gameTreasureSpawnEvent;
    [SerializeField] private LootTable lootTable; 
    private void Awake()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        currentHealth.Value = maxHealth;
        setter = gameObject.GetComponent<AIDestinationSetter>();
        basicEnemyMelee = gameObject.GetComponent<BasicEnemyMelee>();

    }

    public override void OnNetworkSpawn()
    {
        setter = gameObject.GetComponent<AIDestinationSetter>();
        currentHealth.OnValueChanged += (value, newValue) =>
        {
            //Debug.Log(value + " new value " + newValue);
            healthSlider.value = newValue;
        };
        base.OnNetworkSpawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsClient)
            return;
        if (collision.CompareTag("EnemyAwake") && !gameObject.CompareTag("TestDummy"))
        {
            var o = collision.gameObject;
            basicEnemyMelee.Target = o;
            setter.target = o.transform;
        }
        if (!collision.gameObject.CompareTag("Attack"))
            return;
        var attackScript = collision.gameObject.GetComponent<AttackScript>();
        var textMan = textManager.target.GetComponent<TextManager>();
        Debug.Log(attackScript + " " + textMan);
        textMan.testMethod();
        textMan.CreatePopUp(transform.position, attackScript.damage, Color.red);
        Debug.Log("Damage: " + attackScript.damage );
        HealthChange(-attackScript.damage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAwake") && !gameObject.CompareTag("TestDummy"))
        {
            //Debug.Log("Exit Trigger Detected");
            if (setter.target == other.gameObject.transform)
            {
                setter.target = null;
            }
        }
    }
    private void HealthChange(int i)
    {
        currentHealth.Value += i;
        healthSlider.value = currentHealth.Value;
        if (currentHealth.Value <=0)
        {
            ServerSideDespawn();
            
        }
    }

    [ServerRpc]
    private void DeSpawnServerRpc()
    {
        ServerSideDespawn();
        
    }

    private void ServerSideDespawn()
    {
        if (!IsServer)
            return;
        SpawnTreasureObject();
        GetComponent<NetworkObject>().Despawn(this);
    }

    
    private void SpawnTreasureObject()
    {
        var position = transform.position;
        var pos = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        gameTreasureSpawnEvent.TriggerEvent(pos,lootTable);
        
        /*var position = transform.position;
        var pos = new Vector3(position.x, position.y, position.z);
        var treasure = Instantiate(treasurePrefab, position, Quaternion.identity);
        treasure.GetComponent<NetworkObject>().Spawn();*/
    }
}
