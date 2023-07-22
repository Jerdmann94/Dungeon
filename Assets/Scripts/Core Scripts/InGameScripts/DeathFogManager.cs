



using System;
using System.Collections.Generic;
using MyBox;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DeathFogManager : NetworkBehaviour
{
    public int gridSize = 10;
    public float initialSafeZoneSize = 10f;
    public float fogShrinkRate = 1f;
    public float fillPauseDuration = 5f;
    public float phasePauseDuration = 5f;
    public int numPhases = 3;
    public GameObject indicationRingPrefab;
    public Transform indicationRingsParent;
    public Tilemap fogMap;
    public TileBase fogTile;
    private int randomX;
    private int randomY;
    private float safeCenterX;
    private int safeCenterY;
    private float currentSafeZoneSize;
    private float previousRingDiameter;
    private bool[,] safeGrid;
    private bool[,] hasGas;
    private bool isFogShrinking = false;
    private int currentPhase = 0;
    private GameObject indicationRing;

    private static int staticGridSize;
    private static bool[,] _staticSafe;
    

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Not on server");
            return;
        }

        _staticSafe = new bool[gridSize, gridSize];
        staticGridSize = gridSize;
        safeCenterX = gridSize;
        safeCenterY = gridSize;
        currentSafeZoneSize = initialSafeZoneSize;
        previousRingDiameter = initialSafeZoneSize;
        InitializeSafeGrid();
    }

    public void StartFog()
    {
        
        StartCoroutine(ShrinkFog());
        
    }

    private void InitializeSafeGrid()
    {
        _staticSafe = new bool[gridSize, gridSize];
        Debug.Log("grid initing" + _staticSafe);
        safeGrid = new bool[gridSize, gridSize];
        hasGas = new bool[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                safeGrid[x, y] = true;
                hasGas[x, y] = false;
            }
        }

        _staticSafe = safeGrid;
    }

    private void UpdateSafeGrid()
    {
        
        float centerX = (gridSize / 2f) + randomX;
        float centerY = (gridSize / 2f) + randomY;
       
        float radius = currentSafeZoneSize / 2f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
               //float distance = Vector2.Distance(new Vector2(x, y), new Vector2(safeCenterX, safeCenterY));
                safeGrid[x, y] = distance <= radius;
                
            }
        }
        _staticSafe = safeGrid;
    }

    private System.Collections.IEnumerator ShrinkFog()
    {
        
        var percents = GetPercents(numPhases);
        var percentageOfCircle = 0;
        while (currentPhase < numPhases)
        {
            isFogShrinking = true;
            //float fogRingDiameter = initialSafeZoneSize - (currentPhase + 1) * (initialSafeZoneSize / numPhases);
            
           // Debug.Log(percents[currentPhase]);
            
            percentageOfCircle += percents[currentPhase];
            Debug.Log(initialSafeZoneSize-initialSafeZoneSize * ( percentageOfCircle/100f));
            Debug.Log("Percentage of total area to be made into fog" + percentageOfCircle);
            float fogRingDiameter = initialSafeZoneSize-initialSafeZoneSize * ( percentageOfCircle/100f);
            float indicatorRingDiameter = fogRingDiameter;
            GetSafeCenter(fogRingDiameter,previousRingDiameter);
            ClearIndicationRings();
            SpawnIndicationRing(indicatorRingDiameter);
            yield return new WaitForSeconds(phasePauseDuration);
            previousRingDiameter = fogRingDiameter;
            
                
                while (currentSafeZoneSize > fogRingDiameter)
                {
                     currentSafeZoneSize -= fogShrinkRate;
                    UpdateSafeGrid();
                    SpawnFogTiles();
                    yield return new WaitForSeconds(fillPauseDuration);
                }
                currentSafeZoneSize = fogRingDiameter;
            
            
         //   Debug.Log("Moving to phase: " + currentPhase);
            currentPhase++;
        }

        FillRemainingTiles();

        isFogShrinking = false;
    }

    private void GetSafeCenter(float fogRingDiamter, float previousDiameter)
    {
        var fogRingRadius =  fogRingDiamter/2;
        var previousRadius = previousDiameter / 2;
        var ringDif = (int)previousRadius - fogRingRadius;
        var doWeHaveASafeCenter = false;
        var breakCheck = 0;
        while (!doWeHaveASafeCenter)
        {
            var currentRandX = (int)Random.Range(-ringDif, ringDif);
            randomX += currentRandX;
            var currentRandY = (int)Random.Range(-ringDif, ringDif);
            randomY += currentRandY;
            safeCenterX = (int)(gridSize / 2f) + randomX;
            safeCenterY = (int)(gridSize / 2f) + randomY;
            Debug.Log("fogringdiameter " + fogRingDiamter + " previous diameter " + previousDiameter +" ringdif "+ringDif);
            Debug.Log("safex " + safeCenterX + " safey " + safeCenterY + " randx " + randomX +" randy "+randomY+ " curentrandx " + currentRandX +" currendrandy "+currentRandY +" fog radius " + fogRingRadius);

            breakCheck++;
            if (breakCheck > 10)
            {
                Debug.Log("BREAKOUT");
                break;
            }
            // check safe center n s e w for fog

            if (!safeGrid[(int)(safeCenterX + (int)fogRingRadius),safeCenterY])
            {
                continue;
            }
            if (!safeGrid[(int)(safeCenterX - (int)fogRingRadius),safeCenterY])
            {
                continue;
            }
            if (!safeGrid[(int)(safeCenterX),safeCenterY + (int)fogRingRadius])
            {
                continue;
            }
            if (!safeGrid[(int)safeCenterX ,safeCenterY - (int)fogRingRadius])
            {
                continue;
            }
            
            doWeHaveASafeCenter = true;
        }
       
    }

    private void SpawnFogTiles()
    {
        //ClearFogTiles();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!safeGrid[x, y] && !hasGas[x,y])
                {
                    //Vector3 position = new Vector3(x - safeCenterX + 0.5f, y - safeCenterY + 0.5f, 0f);
                    hasGas[x, y] = true;
                    Vector3 position = new Vector3(x - gridSize / 2f, y - gridSize / 2f, 0f);
                    fogMap.SetTile(new Vector3Int(x,y,0),fogTile);
                    SendClientTileInfoClientRpc(x, y);
                    //fogGenerator.ActiveFogTile(new Vector2Int(x,y));

                    //var i =Instantiate(fogTilePrefab, position, Quaternion.identity, fogTilesParent);
                    //i.GetComponent<NetworkObject>().Spawn();
                    //i.transform.SetParent(fogTilesParent);
                }
            }
        }
    }
    
    private void ClearIndicationRings()
    {
        
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Not on server");
            return;
        }
        Debug.Log("deleting indication rings "+ indicationRingsParent.childCount);
        foreach (Transform child in indicationRingsParent)
        {
            child.GetComponent<NetworkObject>().Despawn();
        }
    }

    private void SpawnIndicationRing(float targetSafeZoneSize)
    {
        //ClearIndicationRings();

        float centerX = (gridSize / 2f) + randomX;
        float centerY = (gridSize / 2f) + randomY;
        
       // Debug.Log(indicatorCenterX +" x y "+indicatorCenterY);
        float radius = targetSafeZoneSize / 2f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                //float distance = Vector2.Distance(new Vector2(x, y), new Vector2(indicatorCenterX, indicatorCenterY));
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                bool isInsideRing = Mathf.Abs(distance - radius) < 0.5f;

                if (isInsideRing)
                {
                    Vector3 position = new Vector3(x - gridSize / 2f, y - gridSize / 2f, 0f);
                    //Vector3 position = new Vector3(x - indicatorCenterX+ 0.5f, y - indicatorCenterY + 0.5f, 0f);
                   var i = Instantiate(indicationRingPrefab, position, Quaternion.identity, indicationRingsParent);
                   i.GetComponent<NetworkObject>().Spawn();
                   i.transform.SetParent(indicationRingsParent.transform);
                }
            }
        }
    }

    private void FillRemainingTiles()
    {
        Debug.Log("FILLING REMAINING TILES");
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (safeGrid[x, y])
                {
                    Vector3 position = new Vector3(x - gridSize / 2f + 0.5f, y - gridSize / 2f + 0.5f, 0f);
                    //var i = Instantiate(fogTilePrefab, position, Quaternion.identity, fogTilesParent);
                   // i.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }
    public static List<int> GetPercents(int numPercentages)
    {
        int total = 100;
        int minDifference = 5;

        List<int> percentages = new List<int>();

        // Calculate the target percentage range
        int targetPercentage = total / numPercentages;
        int minPercentage = targetPercentage - minDifference;
        int maxPercentage = targetPercentage + minDifference;

        // Generate percentages within the target range
        int remainingTotal = total;
        for (int i = 0; i < numPercentages - 1; i++)
        {
            int randomPercentage = UnityEngine.Random.Range(minPercentage, maxPercentage + 1);
            percentages.Add(randomPercentage);
            remainingTotal -= randomPercentage;
        }
        percentages.Add(remainingTotal);

        percentages.Sort(); // Sort the list in ascending order
        percentages.Reverse(); // Reverse the list to get largest to smallest order

        return percentages;
    }

    public static bool CheckFogSafety(Vector2 pos)
    {
        //Debug.Log("check fog position " + pos.x +"x y"+ pos.y); 
        int x = Mathf.RoundToInt(staticGridSize / 2f + pos.x);
        int y = Mathf.RoundToInt(staticGridSize / 2f + pos.y);
        return _staticSafe[x, y];
    }
    
    [ClientRpc]
    public void SendClientTileInfoClientRpc(int x, int y)
    {
        fogMap.SetTile(new Vector3Int(x,y,0),fogTile);
    }
}





