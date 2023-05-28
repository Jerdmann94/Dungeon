using System.Collections;
using System.Collections.Generic;
using Core_Scripts.EnemyScripts;
using UnityEngine;

public class GameEnemyMovementRequestListener : MonoBehaviour
{
    public EnemyRequestMovementEvent gameEvent;
    public UnityEventVector3ThreatTableEnemyIdType onEventTriggered;
    void OnEnable()
    {
        gameEvent.AddListener(this);
    }
    void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }
    public void OnEventTriggered(Vector3 pos, List<PlayerThreat> threatTable, string enemyId)
    {
        onEventTriggered.Invoke(pos, threatTable, enemyId);
    }
}
