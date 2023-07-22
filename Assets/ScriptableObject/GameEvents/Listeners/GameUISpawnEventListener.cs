using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameUISpawnEventListener : MonoBehaviour
{
    public GameUISpawnEvent gameEvent;
    public UnityEventGoType onEventTriggered;
    void OnEnable()
    {
        gameEvent.AddListener(this);
    }
    void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }
    public void OnEventTriggered(GameObject ob, OnDropType type)
    {
        onEventTriggered.Invoke(ob,type);
    }
}
