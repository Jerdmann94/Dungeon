using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core_Scripts.EnemyScripts;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    
    
    public ListContainer listContainer;
    public EnemyTargetAcquisitionManager targetAcquisitionManager;
    private LayerMask enemyMask;
    private LayerMask wallMask;
    private LayerMask playerMask;
    private const int AcceptableAttackRange = 25;


    private void Awake()
    {
         enemyMask = LayerMask.GetMask("Enemy");
         wallMask = LayerMask.GetMask("Default");
         playerMask = LayerMask.GetMask("PlayerLayer");
    }

    public void CheckMoveState(Vector3 pos, List<PlayerThreat> threatTable,string id, bool isRanged)
    {
        
        //Debug.Log("Check Move state");
        //CHECK IF THERE IS SOMETHIN TO TARGET
        if (!PatrolCheck(pos)) //IF THERE IS NO TARGET, DO A 1 SPACE PATROL
        {
            //Debug.Log("going to do patrol");
            DoPatrol(pos, id);
        }
        else
        {
            var target = targetAcquisitionManager.TargetAcquisition(pos, threatTable);
            SetTargetForEnemy(target, id);
            var targetPos = target.transform.position;
            //IF WE ARE RANGED AND PATH IS SHORT + WE HAVE LINE OF SIGHT, MOVE BACK OR STAND STILL;
            if (isRanged && MasterPathFinder.RangedCheck(pos,targetPos,wallMask))
            {
                InitateMovementOnEnemy(MasterPathFinder.DoRangedAvoidanceMovement(pos,targetPos,wallMask,enemyMask),id);
            }
            
            
            var sensePath = MasterPathFinder.
                FindPathWithoutWalls(pos, targetPos, wallMask,playerMask ,true);
//            Debug.Log(sensePath.Count + " sense path count");
            if (sensePath.Count >= 20 || sensePath.Count <= 0) return;
   
            var path = MasterPathFinder.
                FindPathWithoutEnemiesAndWalls(pos, targetPos, wallMask, enemyMask, playerMask ,true);

            
            if (path.Count < 20 && path.Count > 0)
            {
                //ADDING THIS TO MAKE SURE WE ARE ALWAYS ON THE CENTER OF A TILE
                var tileCenter =new Vector3
                    (Mathf.RoundToInt(path[0].x),Mathf.RoundToInt(path[0].y),Mathf.RoundToInt(path[0].z));
                InitateMovementOnEnemy(path[0],id);
            }
            //CHECK THE DISTANCE IN A LINE BETWEEN THE TARGET AND MYSELF
            //IF THAT DISTANCE IS CLOSE ENOUGH, SET TO ATTACKING THE TARGET
        }
        
    }


    private bool PatrolCheck(Vector3 pos)
    {
        var anyPlayerCloseEnough = false;
        
        if (listContainer.playerLookUp.Count < 1)
        {
            //Debug.Log("plauyer count is 0");
            return false;
        }
        foreach (var KP in listContainer.playerLookUp)
        {
            //Debug.Log(KP +" "+ listContainer + " "+KP.Value.transform.position);
            if (Vector3.Distance(KP.Value.transform.position, pos) < AcceptableAttackRange)
            {
                anyPlayerCloseEnough = true;
            }
        }
        //Debug.Log("Patrol check bool = " + anyPlayerCloseEnough);
        return anyPlayerCloseEnough;
    }
    private void DoPatrol(Vector3 pos, string id)
    {
        //HAVE TO SEND THIS DATA OVER TO MOVEMENT CONTROLLER
        var target = MasterPathFinder.PatrolOneSpace(pos, wallMask, enemyMask);
        InitateMovementOnEnemy(target, id);
    }
    
    private async void InitateMovementOnEnemy(Vector3 pos, string id)
    {
        if (listContainer.enemyLookUp.ContainsKey(id))
        { 
            listContainer.enemyLookUp[id].enemyMovementController.DoEnemyMovement(pos);
        }
        
    }
    
    private void SetTargetForEnemy(GameObject target, string id)
    {
        if (listContainer.enemyLookUp.ContainsKey(id))
        {
            listContainer.enemyLookUp[id].enemyAttackController.Target = target;
        }
    }
}

public enum MoveState
{
    Patrol,
    Attacking,
    Avoiding
}
