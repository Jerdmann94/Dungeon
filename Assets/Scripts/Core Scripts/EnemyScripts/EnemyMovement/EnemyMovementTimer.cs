using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyMovementTimer : NetworkBehaviour
{
    public float timer;
    private float counter;
    public BasicEnemy enemyController;
    public EnemyRequestMovementEvent movementEvent;
    
    public void GetInitTimer(float t)
    {
        timer = t;
    }

    private void Update()
    {
        /*if (!IsServer)
            return;*/
        if (counter < 0)
        {
            counter = timer;
            AskForMovement();
        }
        else
        {
            counter -= Time.deltaTime;
        }
    }

    private void AskForMovement()
    {
        Debug.Log("Asking for movement");
        // ASK FOR MOVEMENT EVENT 
        movementEvent.TriggerEvent(transform.position, enemyController.threatTable,enemyController.id);
        //GET MOVEMENT BASED ON STATE

    }
}
