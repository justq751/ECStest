using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

// uses a modified version of the ObjectPooler from
// https://www.raywenderlich.com/847-object-pooling-in-unity

public class FXManager : ObjectPooler
{
    // singleton reference
    public static FXManager Instance;

    [Space]

    // explosion to pool
    [SerializeField] private GameObject explosionPrefab;

    // tag is used to find object in pool
    [SerializeField] private string explosionTag = "Explosion";

    // size of the initial object pool
    [SerializeField] private int poolSize = 40;

    // simple Singleton
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // create a new entry in the object pool
        if (explosionPrefab != null)
        {
            ObjectPoolItem explosionPoolItem = new ObjectPoolItem
            {
                objectToPool = explosionPrefab,
                amountToPool = poolSize,
                shouldExpand = true
            };

            // add to the current pool
            itemsToPool.Add(explosionPoolItem);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    // move the pooled explosion prefab into place (particles are set to Play on Awake)
    public void CreateExplosion(Vector3 pos)
    {
        GameObject instance = GetPooledObject(explosionTag);
        if (instance != null)
        {
            instance.SetActive(false);
            instance.transform.position = pos;
            instance.SetActive(true);
        }

    }
}
