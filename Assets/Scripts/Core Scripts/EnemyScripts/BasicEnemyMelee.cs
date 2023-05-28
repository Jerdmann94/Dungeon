using Unity.Netcode;
using UnityEngine;

public class BasicEnemyMelee : NetworkBehaviour
{
    public int attackCD;

    public float attackRange;

    public int damage;

    public LayerMask playerLayer;

    private PlayerController playerController;
    private GameObject target;

    private float timer;


    public GameObject Target
    {
        get => target;
        set
        {
            target = value.transform.parent.parent.gameObject;
            playerController = target.GetComponent<PlayerController>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < attackCD || Target == null)
            return;

        if (CheckRange()) DoDamageToTarget();
    }

    // ALL DAMAGE THINGS SHOULD BE DONE HERE
    private void DoDamageToTarget()
    {
        //Debug.Log("doing melee to player");
        playerController.HealthChangeFromServer(-damage);
        //playerController.health.Value -= damage;
        timer = 0;
    }

    // JUST FOR CHECKING IF OUR TARGET IS WITHIN RANGE AND CAN BE HIT
    private bool CheckRange()
    {
        // CHECK IF LOCATION IS CLOSE ENOUGH TO THIS GAMEOBJECT
        var distance = Target.transform.position - transform.position;

        // ADDING IN BUFFER FOR ATTACK RANGE TILES ARE NOT ALWAYS EXACTLY 1 SPACE AWAY
        if (Mathf.Abs(distance.x) >= attackRange + 0.2f || Mathf.Abs(distance.y) >= attackRange + 0.2f) return false;
        var direction = distance / distance.magnitude;
        var hit = Physics2D.Raycast(transform.position, direction, 5f, playerLayer);
        Debug.DrawRay(transform.position, direction, Color.magenta, 1, false);

        if (!hit) return false;
        //Debug.Log(hit.collider.gameObject + "gameobject + target " + Target);
        // WE HIT THE PLAYER / OUR TARGET
        return hit.collider.gameObject == Target;
    }
}