using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public partial struct RotatingCubeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RotateSpeed>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> localTransform, RefRO<RotateSpeed> rotateSpeed) 
            in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>())
        {
            localTransform.ValueRW = localTransform.ValueRO.RotateY(rotateSpeed.ValueRO.value * SystemAPI.Time.DeltaTime);
        }
    }
}
