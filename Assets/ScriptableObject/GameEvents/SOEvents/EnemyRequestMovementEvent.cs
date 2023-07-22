using System.Collections;
using System.Collections.Generic;
using Core_Scripts.EnemyScripts;
using UnityEngine;
[CreateAssetMenu(menuName ="Game Event/EnemyMovementEvent")]
public class EnemyRequestMovementEvent : ScriptableObject
{
    [SerializeField] private List<GameEnemyMovementRequestListener> listeners = new();
    
    
    public void TriggerEvent(Vector3 pos, List<PlayerThreat> threatTable, string enemyId,bool isRanged)
    {
        for (int i = listeners.Count -1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered(pos, threatTable, enemyId,isRanged);
        }
    }
    public void AddListener(GameEnemyMovementRequestListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(GameEnemyMovementRequestListener listener)
    {
        listeners.Remove(listener);
    }
}

