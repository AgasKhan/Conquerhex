using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour
{
    public class Baker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new MVMNT_FAKE
            {
                movementVector = new float3(UnityEngine.Random.Range(-1f, 1f))
            });
        }
    }
}

public struct MVMNT_FAKE : IComponentData
{
    public float3 movementVector;
}