using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public Transform skater;
    public GameObject railPrefab;
    public GameObject arenaPrefab;
    public GameObject groundPrefab;
    public GameObject obstaclePrefab;

    [Header("segments settings")]
    public float segmentLength = 60f;
    public int segmentOnScreen = 5;
    public int safeZone = 2;

    [Header("segments count")]
    public int minRails = 10;
    public int maxRails = 15;
    // public int arenaCount = 3;
    public int minArena = 1;
    public int maxArena = 4;

    [Header("obstacle settings")]
    public int minObstacles = 7;
    public int maxObstacles = 15;
    public float obstacleY = 2f;

    private int railsToSpawn;
    private int arenasToSpawn;
    private int obstaclesToSpawn;
    private float spawnZ = 0f;
    private int railCounter = 0;
    private int arenaCounter = 0;
    private bool isSpawningArena = false;
    private List<GameObject> activeSegments = new List<GameObject>();
    private List<GameObject> activeGrounds = new List<GameObject>();


    void Start()
    {
        railsToSpawn = Random.Range(minRails, maxRails + 1);
        arenasToSpawn = Random.Range(minArena, maxArena + 1);

        spawnZ = skater.position.z - (safeZone * segmentLength);

        for (int i = 0; i < segmentOnScreen; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (skater.position.z > (spawnZ - (segmentOnScreen * segmentLength)))
        {
            SpawnSegment();
            DeletePreviousSegment();
        }
    }

    void SpawnSegment()
    {
        GameObject segment;
        if (!isSpawningArena)
        {
            segment = Instantiate(railPrefab, Vector3.forward * spawnZ, Quaternion.identity);
            railCounter++;
            if (railCounter >= railsToSpawn)
            {
                isSpawningArena = true;
                railCounter = 0;
                railsToSpawn = Random.Range(minRails, maxRails + 1);
            }
        }
        else
        {
            segment = Instantiate(arenaPrefab, Vector3.forward * spawnZ, Quaternion.identity);
            arenaCounter++;

            // Spawn obstacles
            obstaclesToSpawn = Random.Range(minObstacles, maxObstacles + 1);
            for (int i = 0; i < obstaclesToSpawn; i++)
            {
                float randomX = Random.Range(-23f, 23f);
                float randomZ = spawnZ + Random.Range(-segmentLength / 2f, segmentLength / 2f);
                GameObject obstacle = Instantiate(obstaclePrefab, new Vector3(randomX, obstacleY, randomZ), Quaternion.identity);
                obstacle.transform.parent = segment.transform;
            }

            if (arenaCounter >= arenasToSpawn)
            {
                isSpawningArena = false;
                arenaCounter = 0;
                arenasToSpawn = Random.Range(minArena, maxArena + 1);
            }
            
        }
        activeSegments.Add(segment);

        GameObject ground = Instantiate(groundPrefab, new Vector3(0f, -16.5f, spawnZ), Quaternion.identity);
        activeGrounds.Add(ground);

        spawnZ += segmentLength;
        
    }

    void DeletePreviousSegment()
    {
        if (activeSegments.Count > segmentOnScreen + safeZone)
        {
            Destroy(activeSegments[0]);
            Destroy(activeGrounds[0]);

            activeSegments.RemoveAt(0);
            activeGrounds.RemoveAt(0);
        }
    }

}

