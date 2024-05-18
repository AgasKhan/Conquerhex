using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial class PlayerShootingSystem : SystemBase
{

    public static event EventHandler OnShoot;
    
    protected override void OnCreate()
    {
        RequireForUpdate<PLYR_FAKE>();
    }

    protected override void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Unity.Entities.Entity playerEntity = SystemAPI.GetSingletonEntity<PLYR_FAKE>();
            EntityManager.SetComponentEnabled<Stunned>(playerEntity, true);
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            Unity.Entities.Entity playerEntity = SystemAPI.GetSingletonEntity<PLYR_FAKE>();
            EntityManager.SetComponentEnabled<Stunned>(playerEntity, false);
        }
            
        if(!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }
        SpawnCubesConfig spawnCubesConfig = SystemAPI.GetSingleton<SpawnCubesConfig>();
        
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        //EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(WorldUpdateAllocator);
        foreach ((RefRO<LocalTransform> localTransform, Unity.Entities.Entity entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PLYR_FAKE>().WithDisabled<Stunned>().WithEntityAccess())
        {
             {
                 Unity.Entities.Entity spawnedEntity = entityCommandBuffer.Instantiate(spawnCubesConfig.cubePrefabEntity);
                 //Unity.Entities.Entity spawnedEntity = EntityManager.Instantiate(spawnCubesConfig.cubePrefabEntity);
                 //EntityManager.SetComponentData(spawnedEntity, new LocalTransform
                 SystemAPI.SetComponent(spawnedEntity, new LocalTransform
                 {
                     Position = localTransform.ValueRO.Position,
                     Rotation = quaternion.identity,
                     Scale = 1f
                 });

                 OnShoot?.Invoke(entity, EventArgs.Empty);

             }
             entityCommandBuffer.Playback(EntityManager);
        }
    }
}
