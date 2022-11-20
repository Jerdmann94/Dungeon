using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyMe : NetworkBehaviour
{
   private void Awake()
   {
      Destroy(this, 10f);
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
