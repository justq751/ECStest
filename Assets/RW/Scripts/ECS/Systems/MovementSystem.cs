using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

// abstract class needed to implement a system
public class MovementSystem : ComponentSystem
{
    // This must implement protected override void OnUpdate(), which invokes every frame
    protected override void OnUpdate()
    {
        // Use Entities with a static ForEach to run logic over every Entity in the World
        // WithAll works as a filter, restricts the loop to Entities that have MoveForward data
        // The argument for the ForEach is a lambda function (input parameters) => {expression}
        Entities.WithAll<MoveForward>().ForEach((ref Translation trans, ref Rotation rot, ref MoveForward moveForward) =>
        {
            // The lambda expression calculates the speed relative to one frame, 
            // Then, it multiplies that by the local forward vector, (math.forward(rot.Value)
            trans.Value += moveForward.speed * Time.DeltaTime * math.forward(rot.Value);
        });
    }
}
