using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIRemoveEventListener : MonoBehaviour
{
    public GameUIRemoveEvent gameEvent;
    public UnityEventStringType onEventTriggered;
    void OnEnable()
    {
        gameEvent.AddListener(this);
    }
    void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }
    public void OnEventTriggered(string id, OnDropType type)
    {
        onEventTriggered.Invoke(id,type);
    }
}
