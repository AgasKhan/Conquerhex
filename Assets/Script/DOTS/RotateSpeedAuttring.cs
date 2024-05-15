using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class RotateSpeedAuttring : MonoBehaviour
{
    public float value;

    private class Baker : Baker<RotateSpeedAuttring>
    {
        public override void Bake(RotateSpeedAuttring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DOTS_RotateSpeed
            {
                rotationSpeed = authoring.value
            });
        }
    }
}
