using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using System;
using CheatCommandsPrompt;

public class DOTS_Test : MonoBehaviour
{
    public Vector3 _velocityCalculate;

    private Unity.Entities.Entity _entity;
    private EntityManager _entityManager;

    private void SetEntityVelocity(float3 newVelocity)
    {
        _entityManager.SetComponentData(_entity, new MoveSpeedComponent { Speed = newVelocity });
    }
    
    public class Baker : Baker<DOTS_Test>
    {
        public override void Bake(DOTS_Test authoring)
        {
            authoring. _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            authoring._entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(authoring._entity, new MoveSpeedComponent { Speed = authoring._velocityCalculate });
        }
    }
}

public struct MoveSpeedComponent : IComponentData
{
    public float3 Speed;
}

public partial class MoveSystem : SystemBase
{
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