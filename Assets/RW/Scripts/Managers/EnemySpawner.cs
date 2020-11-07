using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Entities;
using Unity.Collections;

using Random = UnityEngine.Random;
using System;


/// <summary>
/// spawns a swarm of enemy entities offscreen, encircling the player
/// </summary>
public class EnemySpawner : MonoBehaviour
{

    [Header("Spawner")]
    // number of enemies generated per interval
    [SerializeField] private int spawnCount = 30;

    // time between spawns
    [SerializeField] private float spawnInterval = 3f;

    // enemies spawned on a circle of this radius
    [SerializeField] private float spawnRadius = 30f;

    // extra enemy increase each wave
    [SerializeField] private int difficultyBonus = 5;

    [Header("Enemy")]
    // random speed range
    [SerializeField] float minSpeed = 4f;
    [SerializeField] float maxSpeed = 12f;

    // counter
    private float spawnTimer;

    // flag from GameManager to enable spawning
    private bool canSpawn;

    //the one and only manager
    private EntityManager entityManager;

    // enemy mesh and materials
    [SerializeField] private Mesh enemyMesh;
    [SerializeField] private Material enemyMaterial;

    [SerializeField] private GameObject enemyPrefab;
    private Entity enemyEntityPrefab;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);
        //entityManager.Instantiate(enemyEntityPrefab);
        SpawnWave();
    }

    // spawns enemies in a ring around the player
    private void SpawnWave()
    {
        // Declare a new NativeArray, with up to the spawnCount elements. 
        // Allocator.Temp indicates the NativeArray won’t need to persist once the setup is complete
        NativeArray<Entity> enemyArray = new NativeArray<Entity>(spawnCount, Allocator.Temp);

        // Each iteration instantiates an Entity and stores it in enemyArray
        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemyArray[i] = entityManager.Instantiate(enemyEntityPrefab);
            // find a 3D position
            entityManager.SetComponentData(enemyArray[i], new Translation { Value = RandomPointOnCircle(spawnRadius) });
            entityManager.SetComponentData(enemyArray[i], new MoveForward { speed = Random.Range(minSpeed, maxSpeed) });
        }
        // frees any temporarily allocated memory
        enemyArray.Dispose();
        // increment the spawnCount on each wave to make the game progressively harder
        spawnCount += difficultyBonus;
    }

    // get a random point on a circle with given radius
    private float3 RandomPointOnCircle(float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * radius;

        // return random point on circle, centered around the player position
        return new float3(randomPoint.x, 0.5f, randomPoint.y) + (float3)GameManager.GetPlayerPosition();
    }

    // signal from GameManager to begin spawning
    public void StartSpawn()
    {
        canSpawn = true;
    }

    private void Update()
    {
        // disable if the game has just started or if player is dead
        if (!canSpawn || GameManager.IsGameOver())
        {
            return;
        }

        // count up until next spawn
        spawnTimer += Time.deltaTime;

        // spawn and reset timer
        if (spawnTimer > spawnInterval)
        {
            SpawnWave();
            spawnTimer = 0;
        }
    }
}
