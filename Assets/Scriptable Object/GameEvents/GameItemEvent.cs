using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Game Event/GameItemListEvent")]
[Serializable]
public class GameItemEvent : ScriptableObject
{
    private List<GameItemEventListener> listeners = new List<GameItemEventListener>();
    public void TriggerEvent(List<GameItem> items)
    {
        for (int i = listeners.Count -1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered(items);
        }
    }
    public void AddListener(GameItemEventListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(GameItemEventListener listener)
    {
        listeners.Remove(listener);
    }
}