using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Game Event/GameTreasureSpawnEvent")]
[Serializable]
public class GameTreasureSpawnEvent : ScriptableObject
{
    private List<GameTreasureSpawnEventListener> listeners = new List<GameTreasureSpawnEventListener>();
    public void TriggerEvent(Vector3 pos, LootTable lootTable)
    {
        for (int i = listeners.Count -1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered(pos, lootTable);
        }
    }
    public void AddListener(GameTreasureSpawnEventListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(GameTreasureSpawnEventListener listener)
    {
        listeners.Remove(listener);
    }
}