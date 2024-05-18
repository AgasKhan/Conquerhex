using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
//[WithNone(typeof(PLYR_FAKE))]
[WithAll(typeof(RotatingCube))]
public partial struct RotatingCubeJob : IJobEntity
{
    public float deltaTime;
    
    
    public void Execute(ref LocalTransform localTransform, in RotateSpeed rotateSpeed)
    {
        float power = 1f;

        for (int i = 0; i < 100000; i++)
        {
            power *= 2;
            power /= 2;
        }
        localTransform = localTransform.RotateY(rotateSpeed.value * deltaTime*power);
    }
}
