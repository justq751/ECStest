using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// interface for implementing general-purpose components
// any implementation must be a struct
[GenerateAuthoringComponent]
public struct MoveForward : IComponentData
{
    public float speed;
}
