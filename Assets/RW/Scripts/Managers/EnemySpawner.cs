﻿using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Entities;

using Random = UnityEngine.Random;


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


    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // associate data types together
        EntityArchetype archetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld));

        // pass the archetype for initialization
        Entity entity = entityManager.CreateEntity(archetype);

        // add data and specific values
        entityManager.AddComponentData(entity, new Translation { Value = new float3(-3f, 0.5f, 5f) });
        entityManager.AddComponentData(entity, new Rotation { Value = quaternion.EulerXYZ(new float3(0f, 45f, 0f)) });
        entityManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = enemyMesh,
            material = enemyMaterial
        });
    }

    // spawns enemies in a ring around the player
    private void SpawnWave()
    {

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
