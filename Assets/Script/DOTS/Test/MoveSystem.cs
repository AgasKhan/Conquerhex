using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using System;
/*
public partial class MoveSystem : SystemBase
{

    public Dictionary<int, DOTS_Test> moveReference = new Dictionary<int, DOTS_Test>();

    unsafe protected override void OnStartRunning()
    {
        Entities.ForEach((ref MoveSpeedComponent moveSpeed,in Unity.Entities.Entity entity) =>
            {
                fixed (MoveSpeedComponent* pSpeed = &moveSpeed)
                {
                    moveReference[entity.Index].componentSpeed = pSpeed;

                    moveReference.Remove(entity.Index);
                }
            }).WithoutBurst().Run();
    }


    [BurstCompile]
    unsafe protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        Entities
            .ForEach((ref LocalTransform localTransform, in MoveSpeedComponent moveSpeed) =>
            {
                localTransform.Position += (moveSpeed.Speed) * deltaTime;
            }).ScheduleParallel();
    }
}
*/