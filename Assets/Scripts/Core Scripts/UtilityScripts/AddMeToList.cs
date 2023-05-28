using UnityEngine;

public class AddMeToList : MonoBehaviour
{
    public PlayerSpawnPointList list;

    // Start is called before the first frame update
    private void Awake()
    {
        list.spawnPods.Add(gameObject);
        list.spawnerUser.Add(false);
    }

    private void OnDisable()
    {
        list.spawnPods.Clear();
        list.spawnerUser.Clear();
    }
}