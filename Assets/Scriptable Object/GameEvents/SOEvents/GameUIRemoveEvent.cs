using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Game Event/UI Remove Event")]
public class GameUIRemoveEvent : ScriptableObject
{
   [SerializeField] private List<GameUIRemoveEventListener> listeners = new List<GameUIRemoveEventListener>();
    public void TriggerEvent(string id, OnDropType type)
    {
//        Debug.Log("In Event, type is " + type + " for this many listeners " + listeners.Count);
        for (int i = listeners.Count -1; i >= 0; i--)
        {
            //Debug.Log(listeners[i].gameObject.name);
            //Debug.Log(i);
            listeners[i].OnEventTriggered(id, type);
        }
    }
    public void AddListener(GameUIRemoveEventListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(GameUIRemoveEventListener listener)
    {
        listeners.Remove(listener);
    }
}
