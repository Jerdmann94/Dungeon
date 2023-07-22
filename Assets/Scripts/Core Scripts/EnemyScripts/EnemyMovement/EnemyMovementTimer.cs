using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyMovementTimer : NetworkBehaviour
{
    public bool isRanged;
    public float speed;
    public float buffTimer;
    private float counter;
    public EnemyBrain enemyBrainController;
    public EnemyRequestMovementEvent movementEvent;
    
    //BASE SPEED OF 10, WHICH IS 1 MOVE PER 2 SECONDS
    // AS SPEED INCREASES TIME BETWEEN MOVE CALLS SHORTENS
    
    public void GetInitTimer(float t)
    {
        speed = t;
    }

    public void SetBuffTimer(float t)
    {
        buffTimer = t;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        // THIS WILL DISABLE MOVEMENT TILL GAME STARTS
        // REMOVE IT TO TEST MOVEMENT MECHANICS WITHOUT NEEDING CLIENT
        if (!GameMaster.gameStartStatic)
        {
            return;
        }
        
        counter += Time.deltaTime;
        if (counter < SpeedCalculator.GetSpeedTimer(speed + buffTimer)) return;
      //  Debug.Log(speed +" buff timer " + buffTimer);
//        Debug.Log(counter + " counter ");
       // Debug.Log("get speed "+SpeedCalculator.GetSpeedTimer(speed + buffTimer));
        AskForMovement();
        counter = 0;

    }

    private void AskForMovement()
    {
        //Debug.Log("Asking for movement");
        // ASK FOR MOVEMENT EVENT 
        movementEvent.TriggerEvent(transform.position, enemyBrainController.threatTable,enemyBrainController.id,isRanged);
        //GET MOVEMENT BASED ON STATE

    }
}
