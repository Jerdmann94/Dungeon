
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "EnemyData/Attacks/BasicMelee")]
public class EnemyAttack : ScriptableObject
{
    public float coolDown;
    public int attackRange;
    public int lowAttack;
    public int highAttack;
    protected  LayerMask wallLayer;
    

    public virtual void DoAttack(GameObject target, Transform ourPosition)
    {
        if (target.CompareTag("Player"))
        {
            target.GetComponent<PlayerController>().HealthChangeFromServer(-Random.Range(lowAttack,highAttack));
        }
    }

    protected void OnEnable()
    {
        wallLayer = LayerMask.GetMask("Default");
    }

    public virtual bool CheckRangeForAttack(GameObject target,Vector3 ourPosition,LayerMask playerLayer,LayerMask wallLayer)
    {
        // CHECK IF LOCATION IS CLOSE ENOUGH TO THIS GAMEOBJECT
        var distance = target.transform.position - ourPosition;

        // ADDING IN BUFFER FOR ATTACK RANGE TILES ARE NOT ALWAYS EXACTLY 1 SPACE AWAY
        if (Mathf.Abs(distance.x) >= attackRange + 0.2f || Mathf.Abs(distance.y) >= attackRange + 0.2f) return false;
        var direction = distance / distance.magnitude;
        var hit = Physics2D.Raycast(ourPosition, direction, attackRange, playerLayer);
        Debug.DrawRay(ourPosition, direction, Color.magenta, 1, false);

        if (!hit) return false;
        //Debug.Log(hit.collider.gameObject + "gameobject + target " + Target);
        // WE HIT THE PLAYER / OUR TARGET
        return hit.collider.gameObject == target;
    }
}

