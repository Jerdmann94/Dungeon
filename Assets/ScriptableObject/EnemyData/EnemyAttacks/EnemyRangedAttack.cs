using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "EnemyData/Attacks/BasicRanged")]


public class EnemyRangedAttack : EnemyAttack
{
    public GameObject rangedProjectile;
    public override void DoAttack(GameObject target, Transform ourPosition)
    {
        var rp = Instantiate(rangedProjectile, ourPosition.position, Quaternion.identity);
        rp.GetComponent<RangedProjectileScript>().GoToAndDie(target.transform.position);
        if (target.CompareTag("Player"))
        {
            target.GetComponent<PlayerController>().HealthChangeFromServer(-Random.Range(lowAttack,highAttack));
        }
    }

    public override bool CheckRangeForAttack(GameObject target,Vector3 ourPosition,LayerMask playerLayer,LayerMask wallLayer)
    {
//        Debug.Log(target.name + target.transform.position + " name and postition of target");
        var distance = target.transform.position - ourPosition;
        var direction = distance / distance.magnitude;
        //SINCE THIS IS RANGED, WE HAVE TO CHECK FOR WALLS BEFORE ATTACKING WITH NORMAL CHECK
        var hit = Physics2D.Raycast(ourPosition, direction, attackRange, wallLayer);
   //     Debug.DrawRay(ourPosition, direction, Color.blue, 2, false);
//        Debug.Log("Did the ray cast for range hit a wall " + hit);
        return !hit && base.CheckRangeForAttack(target, ourPosition, playerLayer,wallLayer);
    }
}
    