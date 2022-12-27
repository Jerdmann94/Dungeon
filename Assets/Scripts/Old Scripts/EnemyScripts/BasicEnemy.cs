
using Pathfinding;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemy : NetworkBehaviour
{
    public Slider healthSlider;
    public int maxHealth;
    public GameObject treasurePrefab;
    private int currentHealth;
    [SerializeField] private AIDestinationSetter setter;
    private BasicEnemyMelee basicEnemyMelee;

    private void Awake()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        currentHealth = maxHealth;
        setter = gameObject.GetComponent<AIDestinationSetter>();
        basicEnemyMelee = gameObject.GetComponent<BasicEnemyMelee>();

    }

    public override void OnNetworkSpawn()
    {
        setter = gameObject.GetComponent<AIDestinationSetter>();
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
        PopUpText.CreatePopUp(transform.position, attackScript.damage, Color.red);
        Debug.Log("Damage: " + attackScript.damage );
        HealthChange(-attackScript.damage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAwake") && !gameObject.CompareTag("TestDummy"))
        {
            Debug.Log("Exit Trigger Detected");
            if (setter.target == other.gameObject.transform)
            {
                setter.target = null;
            }
        }
    }
    private void HealthChange(int i)
    {
        currentHealth += i;
        healthSlider.value = currentHealth;
        if (currentHealth <=0)
        {
            DeSpawnServerRpc();
            
        }
    }

    [ServerRpc]
    private void DeSpawnServerRpc()
    {
        SpawnTreasureObjectServerRpc();
        GetComponent<NetworkObject>().Despawn(this);
    }

    [ServerRpc]
    private void SpawnTreasureObjectServerRpc()
    {
        var position = transform.position;
        var pos = new Vector3(position.x, position.y, position.z);
        var treasure =  Instantiate(treasurePrefab, position,Quaternion.identity)as GameObject;
    }
}
