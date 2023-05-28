using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTreasureSpawnEventListener : MonoBehaviour
{
    public GameTreasureSpawnEvent gameEvent;
    public UnityEventVector3LootTable onEventTriggered;
    void OnEnable()
    {
        gameEvent.AddListener(this);
    }
    void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }
    public void OnEventTriggered(Vector3 pos, LootTable lootTable)
    {
        onEventTriggered.Invoke(pos,lootTable);
    }
}
