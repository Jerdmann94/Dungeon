using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticReference/ListContainer")]
public class ListContainer : ScriptableObject
{
    public List<TreasureScript> treasureScripts = new List<TreasureScript>();
    public List<GameItem> itemsInServer = new List<GameItem>();
    public List<DropOnMe> clientCollectionsThatNeedTreasureScript;
    public TreasureScript lastTreasureScript;


    public void Init()
    {
        treasureScripts = new List<TreasureScript>();
        itemsInServer = new List<GameItem>();
    }

    public GameObject FindTreasureObject(string id)
    {
        GameObject treasure = null;

        treasure = treasureScripts.Find(script => script.id == id
        ).gameObject;
        return treasure;
    }
    public TreasureScript FindTreasure(string id)
    {
        
        Debug.Log("ID used to find treasure = " + id);
        TreasureScript treasure = null;

        treasure = treasureScripts.Find(script => script.id == id
        );
        return treasure;
    }

    public GameItem FindItem(string id)
    {
        GameItem g = null;

        g = itemsInServer.Find(script => script.id == id
        );
        return g;
    }

    public void GiveTreasureScript(TreasureScript treasureScript)
    {
        lastTreasureScript = treasureScript;
        foreach (var drop in clientCollectionsThatNeedTreasureScript)
        {
            drop.treasureScript = treasureScript;
        }
    }

    public TreasureScript ReturnTreasureScript()
    {
        return lastTreasureScript;
    }

    public TreasureScript DoesAChestAlreadyExistThere(Vector3 v3)
    {
        TreasureScript ts = null;
        if (treasureScripts.Count == 0)
        {
            return null;
        }
        foreach (var treasure in treasureScripts)
        {
            if (treasure.gameObject.transform.position == v3)
            {
                ts = treasure;
            }
        }

        return ts;
    }
}
