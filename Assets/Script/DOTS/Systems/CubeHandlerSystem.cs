using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct CubeHandlerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        //foreach((RefRW<LocalTransform> localTransform, RefRO<RotateSpeed> rotateSpeed, RefRO<MVMNT_FAKE> movement)
        //        in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>, RefRO<MVMNT_FAKE>>().WithAll<RotatingCube>())
        foreach(RotatingMovingCubeAspect rotatingMovingCubeAspect in SystemAPI.Query<RotatingMovingCubeAspect>())
        {
            rotatingMovingCubeAspect.MoveAndRotate(SystemAPI.Time.DeltaTime);

        }
    }
}
