using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FacePlayerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }
        // store the player’s location
        float3 playerPos = (float3)GameManager.GetPlayerPosition();
        //loop through all Entities
        Entities.ForEach((Entity entity, ref Translation trans, ref Rotation rot) =>
        {
            // calculate the vector to the player
            float3 direction = playerPos - trans.Value;
            direction.y = 0f;
            // to set the correct heading
            rot.Value = quaternion.LookRotation(direction, math.up());
        });
    }
}
