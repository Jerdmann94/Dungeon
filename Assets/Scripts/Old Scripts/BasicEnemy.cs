using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasicEnemy : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth;
    private int currentHealth;
    [SerializeField] private AIDestinationSetter setter;

    private void Awake()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAwake"))
        {
            Debug.Log(setter );
            setter.target = collision.gameObject.transform;
        }
        if (!collision.gameObject.CompareTag("Attack"))
            return;
        var attackScript = collision.gameObject.GetComponent<AttackScript>();
        PopUpText.CreatePopUp(transform.position, attackScript.damage, Color.red);
        Debug.Log("Damage: " + attackScript.damage );
        HealthChange(-attackScript.damage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAwake"))
        {
            Debug.Log("Exit Trigger Detected");
            if (setter.target == other.gameObject.transform)
            {
                setter.target = null;
            }
        }
    }
    private void HealthChange(int i)
    {
        currentHealth += i;
        healthSlider.value = currentHealth;
        if (currentHealth <=0)
        {
            DeSpawnServerRpc();
            
        }
    }

    [ServerRpc]
    private void DeSpawnServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(this);
    }
}
