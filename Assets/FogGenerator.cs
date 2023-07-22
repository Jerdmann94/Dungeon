using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FogGenerator : MonoBehaviour
{
    public int gridSize;
    public GameObject fogTilePrefab;
    public GameObject tileParent;
    public ListContainer listContainer;
    
    
    public void SpawnFogTiles()
    {
        Debug.Log("child count for fog tiles" + transform.childCount);
        while (transform.childCount > 0)
        {
            
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        Debug.Log("Child count after destroy, should be 0" + transform.childCount);
        listContainer.fogGrid = new Dictionary<Vector2Int, GameObject>();
        int halfGridSize = gridSize / 2;

        for (int x = -halfGridSize; x <= halfGridSize; x++)
        {
            for (int y = -halfGridSize; y <= halfGridSize; y++)
            {
                // Calculate the position based on the grid size
                float xPos = x;
                float yPos = y;

                Vector2 gridPosition = new Vector2(xPos, yPos);
                GameObject fogTile = Instantiate(fogTilePrefab, gridPosition, Quaternion.identity,tileParent.transform);
                listContainer.fogGrid.Add(Vector2Int.RoundToInt(gridPosition), fogTile);
                
                fogTile.name = "FogTile"+x+"" + y;
                fogTile.SetActive(false);
            }
        }
    }

   

    public void ActiveFogTile(Vector2Int vector2Int)
    {
        listContainer.fogGrid[vector2Int].SetActive(true);
    }
}
