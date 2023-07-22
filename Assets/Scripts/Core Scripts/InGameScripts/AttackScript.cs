using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public int damage;
    public List<GameObject> hitList = new();

    public NetworkObject obj;
    public void CheckForLos()
    {
        var transform1 = transform;
        var transform2 = transform1.parent.transform;
        var position = transform1.position;
        var position2 = transform2.position;
        var distance = position - position2;
        var direction = distance / distance.magnitude;

        var distF = Vector3.Distance(new Vector3(position.x, position.y, 0), position2);
        var hit = Physics2D.Raycast(position2, direction, .9f, 1);
        if (hit)
            //Instantiate(losSpawn, position, Quaternion.identity);
            //Debug.Log(hit.collider.gameObject.name+ "   " + distF);
            gameObject.SetActive(false);
        //else
            //Debug.Log(" raycast did not hit");
    }

    public void Init(int dam)
    {
        damage = dam;
        obj.Spawn();
        Destroy(this.gameObject, 1f);
    }
}