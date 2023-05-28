using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Game Event/UI Spawn Event")]
public class GameUISpawnEvent : ScriptableObject
{
    private List<GameUISpawnEventListener> listeners = new List<GameUISpawnEventListener>();
    public void TriggerEvent(GameObject go, OnDropType type)
    {
        for (int i = listeners.Count -1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered(go, type);
        }
    }
    public void AddListener(GameUISpawnEventListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(GameUISpawnEventListener listener)
    {
        listeners.Remove(listener);
    }
}