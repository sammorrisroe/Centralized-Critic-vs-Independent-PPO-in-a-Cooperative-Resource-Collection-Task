using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private float minSpawnTime = 2f;
    [SerializeField] private float maxSpawnTime = 5f;
    [SerializeField] private int minSpawnNum = 1;
    [SerializeField] private int maxSpawnNum = 3;
    [SerializeField] private int initialSpawnNum = 3;
    [SerializeField] private int maxResources = 20;

    [Header("Map Bounds")]
    [SerializeField] private float mapMinX = -14f;
    [SerializeField] private float mapMaxX = 14f;
    [SerializeField] private float mapMinZ = -14f;
    [SerializeField] private float mapMaxZ = 14f;

    [Header("Edge Spawn Settings")]
    [SerializeField] private float spawnY = 0.5f;
    [SerializeField] private float edgeInset = 0f; 
    

    private float spawnTime;
    private float timeSinceLastSpawn = 0f;
    private int spawnNum;
    private int currentResources = 0;

    private bool isResetting = false;
    private readonly List<GameObject> liveResources = new List<GameObject>();

    private void Start()
    {
        ResetResources();
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn > spawnTime)
        {
            timeSinceLastSpawn = 0f;

            for (int i = 0; i < spawnNum; i++)
            {
                if (currentResources < maxResources)
                {
                    SpawnResource();
                }
                else
                {
                    break;
                }
            }

            spawnNum = Random.Range(minSpawnNum, maxSpawnNum + 1);
            spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    public void ResetResources()
    {
        isResetting = true;

        for (int i = liveResources.Count - 1; i >= 0; i--)
        {
            if (liveResources[i] != null)
            {
                Destroy(liveResources[i]);
            }
        }

        liveResources.Clear();
        currentResources = 0;
        timeSinceLastSpawn = 0f;

        spawnNum = Random.Range(minSpawnNum, maxSpawnNum + 1);
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);

        for (int i = 0; i < initialSpawnNum; i++)
        {
            SpawnResource();
        }

        isResetting = false;
    }

    private void SpawnResource()
    {
        if (currentResources >= maxResources)
            return;

        Vector3 spawnPos = GetRandomEdgeSpawnPosition();

        GameObject resource = Instantiate(
            resourcePrefab,
            spawnPos,
            Quaternion.identity
        );

        currentResources++;
        liveResources.Add(resource);

        Resource resourceScript = resource.GetComponent<Resource>();
        if (resourceScript != null)
        {
            resourceScript.Init(this);
        }
    }

    private Vector3 GetRandomEdgeSpawnPosition()
    {
        int edge = Random.Range(0, 4);

        float x = 0f;
        float z = 0f;

        switch (edge)
        {
            case 0: // Left edge
                x = mapMinX + edgeInset;
                z = Random.Range(mapMinZ, mapMaxZ);
                break;

            case 1: // Right edge
                x = mapMaxX - edgeInset;
                z = Random.Range(mapMinZ, mapMaxZ);
                break;

            case 2: // Bottom edge
                x = Random.Range(mapMinX, mapMaxX);
                z = mapMinZ + edgeInset;
                break;

            case 3: // Top edge
                x = Random.Range(mapMinX, mapMaxX);
                z = mapMaxZ - edgeInset;
                break;
        }

        return new Vector3(x, spawnY, z);
    }

    public void OnResourceDestroyed(GameObject resource)
    {
        if (isResetting) return;

        currentResources = Mathf.Max(0, currentResources - 1);
        liveResources.Remove(resource);
    }

    public Vector3 GetNearestResourceRelativePosition(Vector3 fromPosition)
    {
        GameObject nearest = null;
        float bestDistSqr = float.MaxValue;

        foreach (GameObject resource in liveResources)
        {
            if (resource == null) continue;

            float distSqr = (resource.transform.position - fromPosition).sqrMagnitude;
            if (distSqr < bestDistSqr)
            {
                bestDistSqr = distSqr;
                nearest = resource;
            }
        }

        if (nearest == null)
        {
            return Vector3.zero;
        }

        return nearest.transform.position - fromPosition;
    }
}