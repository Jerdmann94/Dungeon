using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;

public class AoeParent : NetworkBehaviour
{
    public List<AttackScript> children;

    public override async void OnNetworkSpawn()
    {
        foreach (var child in children) child.CheckForLos();


        if (IsServer)
        {
            await Task.Delay(600);
            GetComponent<NetworkObject>().Despawn();
        }

    }
    
    public void SetChildDamage(int damage)
    {
        foreach (var child in children) child.damage = damage;
    }
}