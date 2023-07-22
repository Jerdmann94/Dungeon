using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataCollection : MonoBehaviour
{
    public List<EnemyData> ED;

    public EnemyData GetEnemyData(string name)
    {
        foreach (var enemyData in ED)
        {
            if (enemyData.name == name)
            {
                return enemyData;
            }
        }

        return null;
    }
}
