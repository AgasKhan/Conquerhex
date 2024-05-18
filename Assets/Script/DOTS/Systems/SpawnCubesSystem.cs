using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class SpawnCubesSystem : SystemBase
{
    protected override void OnCreate()
    {
       RequireForUpdate<SpawnCubesConfig>();
    }

    protected override void OnUpdate()
    {
        this.Enabled = false;

        SpawnCubesConfig spawnCubesConfig = SystemAPI.GetSingleton<SpawnCubesConfig>();

        for (int i = 0; i < spawnCubesConfig.amountToSpawn; i++)
        {
            Unity.Entities.Entity spawnedEntity = EntityManager.Instantiate(spawnCubesConfig.cubePrefabEntity);
            //EntityManager.SetComponentData(spawnedEntity, new LocalTransform
            SystemAPI.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = new float3(UnityEngine.Random.Range(-10, +5), .6f, UnityEngine.Random.Range(-4f, +7)),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
        }
    }
}
