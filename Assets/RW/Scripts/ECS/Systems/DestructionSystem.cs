using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class DestructionSystem : ComponentSystem
{
    // Minimum value to register a collision
    float thresholdDistance = 2f;
    protected override void OnUpdate()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }

        //On each frame, cache the player’s position
        float3 playerPosition = (float3)GameManager.GetPlayerPosition();

        Entities.WithAll<EnemyTag>().ForEach((Entity enemy, ref Translation enemyPos) =>
        {
            // Disregard the player y value
            playerPosition.y = enemyPos.Value.y;

            // Check if the player and enemy are close enough
            if (math.distance(enemyPos.Value, playerPosition) <= thresholdDistance)
            {
                FXManager.Instance.CreateExplosion(enemyPos.Value);
                //FXManager.Instance.CreateExplosion(playerPosition);
                //GameManager.EndGame();

                // Remove the Entity. This is an Entity Command Buffer that waits for a safe time to remove any Entities or data
                PostUpdateCommands.DestroyEntity(enemy);
            }

            // Check collisions between the enemy with the player bullets
            float3 enemyPosition = enemyPos.Value;
            Entities.WithAll<BulletTag>().ForEach((Entity bullet, ref Translation bulletPos) =>
            {
                if (math.distance(enemyPosition, bulletPos.Value) <= thresholdDistance)
                {
                    PostUpdateCommands.DestroyEntity(enemy);
                    PostUpdateCommands.DestroyEntity(bullet);
                    FXManager.Instance.CreateExplosion(enemyPosition);
                    GameManager.AddScore(1);
                }
            });
        });
    }
}
