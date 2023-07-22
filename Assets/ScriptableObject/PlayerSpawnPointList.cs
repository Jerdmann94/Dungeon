using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSpawnPointList : ScriptableObject
{
   
    public List<GameObject> spawnPods;
    public List<bool> spawnerUser;
}
