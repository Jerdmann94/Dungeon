using System.Collections;
using System.Collections.Generic;
using Core_Scripts.EnemyScripts;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public ListContainer listContainer;

    public LayerMask enemyMask;
    public LayerMask wallMask;
    private const int AcceptableAttackRange = 25;
    public void CheckMoveState(Vector3 pos, List<PlayerThreat> threatTable,string id)
    {
        Debug.Log("Check Move state");
        //CHECK IF THERE IS SOMETHIN TO TARGET
        if (!PatrolCheck(pos)) //IF THERE IS NO TARGET, DO A 1 SPACE PATROL
        {
            Debug.Log("going to do patrol");
            DoPatrol(pos, id);
        }
        else
        {
            var targetPos = TargetAcquisition(pos, threatTable);
            //CHECK THE DISTANCE IN A LINE BETWEEN THE TARGET AND MYSELF
            //IF THAT DISTANCE IS CLOSE ENOUGH, SET TO ATTACKING THE TARGET
        }
        
    }

    private void DoPatrol(Vector3 pos, string id)
    {
        //HAVE TO SEND THIS DATA OVER TO MOVEMENT CONTROLLER
       var target = MasterPathFinder.PatrolOneSpace(pos, wallMask, enemyMask);
       InitateMovementOnEnemy(target, id);
    }

    private bool PatrolCheck(Vector3 pos)
    {
        var anyPlayerCloseEnough = false;
        
        if (listContainer.playerLookUp.Count < 1)
        {
            Debug.Log("plauyer count is 0");
            return anyPlayerCloseEnough;
        }
        foreach (var KP in listContainer.playerLookUp)
        {
            if (Vector3.Distance(KP.Value.transform.position, pos) < AcceptableAttackRange)
            {
                anyPlayerCloseEnough = true;
            }
        }
        Debug.Log("Do patrol check " + anyPlayerCloseEnough);
        return anyPlayerCloseEnough;
    }

    private Vector3 TargetAcquisition(Vector3 pos, List<PlayerThreat> threatTable)
    {
        
        foreach (var KP in listContainer.playerLookUp)
        {
            
        }

        return new Vector3();
    }

    private async void InitateMovementOnEnemy(Vector3 pos, string id)
    {
        await listContainer.enemyLookUp[id].enemyMovementController.DoEnemyMovement(pos);
    }
    
}

public enum MoveState
{
    Patrol,
    Attacking,
    Avoiding
}
