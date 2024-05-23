using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using System;

public partial class MoveSystem : SystemBase
{
    public event EventHandler OnIdle;
    public event EventHandler OnMove;

    [BurstCompile]
    unsafe protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        Entities
            .ForEach((ref LocalTransform localTransform, in MoveSpeedComponent moveSpeed) =>
            {
                localTransform.Position += ((float3)(moveSpeed.speed)) * deltaTime;
                //MoveTr.MyStaticFixedUpdate(localTransform);

                OnIdle.Invoke(this, EventArgs.Empty);

            }).ScheduleParallel();
    }
}
