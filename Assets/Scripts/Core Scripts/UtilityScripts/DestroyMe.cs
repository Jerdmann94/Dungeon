using Unity.Netcode;

public class DestroyMe : NetworkBehaviour
{
    public float destoryTimer = 10f;
    private void Awake()
    {
        if (destoryTimer > 1f)
        {
            Destroy(this.gameObject,10f);
        }
        else
        {
            Destroy(this.gameObject, destoryTimer);
        }
        
    }

    public void DestroyThis()
    {
        DestroyMeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyMeServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyParentServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(gameObject);
    }
}