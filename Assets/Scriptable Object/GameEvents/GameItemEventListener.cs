using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameItemEventListener : MonoBehaviour
{
    public GameItemEvent gameEvent;
    public UnityEventGameItemList onEventTriggered;
    void OnEnable()
    {
        gameEvent.AddListener(this);
    }
    void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }
    public void OnEventTriggered(List<GameItem> items)
    {
        onEventTriggered.Invoke(items);
    }
}
