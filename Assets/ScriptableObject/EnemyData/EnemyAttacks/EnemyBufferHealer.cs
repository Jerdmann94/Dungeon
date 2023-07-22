using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "EnemyData/Attacks/BuffHeal")]
public class EnemyBufferHealer : EnemyAttack
{
    public LayerMask enemyMask;
    public LayerMask wallAndEnemy;
    public EnemyBuffType enemyBuffType;
    public EffectData effectData;
    //CAST BUFF HEAL ON ENEMY

    
    public override void DoAttack(GameObject target, Transform ourPosition)
    {
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(ourPosition.position, attackRange, enemyMask);

        foreach (Collider2D collider in colliders)
        {
            // Perform a linecast to check line of sight
            if (Vector3Int.RoundToInt(collider.transform.position) == Vector3Int.RoundToInt(ourPosition.position))
            {
                continue;
            }
            if (!HasLineOfSight(collider.transform, ourPosition.position)) continue;
            //do the buff/heal
            switch (enemyBuffType)
            {
                case EnemyBuffType.Healing:
                    var healing = Random.Range(lowAttack, highAttack);
                    collider.gameObject.GetComponent<EnemyBrain>().HealthChange(healing);
                    break;
                case EnemyBuffType.Haste:
                    collider.GetComponent<IEffectable>().AddEffect
                    (new EffectObj(effectData.damage,effectData.speed,
                        effectData.halfLife,effectData.tickRate, effectData.oneTimeEffect));
                    break;
            }
            return;
        }
    }

    //GET ALL ENEMIES WITHIN RANGE, PICK ONE AND GIVE IT BUFF/ HEAL
    public override bool CheckRangeForAttack(GameObject target, Vector3 ourPosition, LayerMask playerLayer, LayerMask wallLayer)
    {
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(ourPosition, attackRange, enemyMask);
        foreach (Collider2D collider in colliders)
        {
//            Debug.Log("collider positiun" + Vector3Int.RoundToInt(collider.transform.position) + 
 //           " Our position " + Vector3Int.RoundToInt(ourPosition));
            if (Vector3Int.RoundToInt(collider.transform.position) == Vector3Int.RoundToInt(ourPosition))
            {
                continue;
            }
            // Perform a linecast to check line of sight
            if (HasLineOfSight(collider.transform,ourPosition))
            {
                // there is atleast 1 enemy within range
          //      Debug.Log("atleast one target within heal/buff");
                return true;
            }
        }

        return false;
    }
   

    private bool HasLineOfSight(Transform target, Vector3 ourPos)
    {
       
         var hit = Physics2D.Linecast(ourPos, target.position, wallAndEnemy);


         return hit == target;
    }
}
public enum EnemyBuffType
{
    Healing,
    Haste,
    Damage,
}