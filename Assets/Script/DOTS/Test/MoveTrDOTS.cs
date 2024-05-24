using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

public class MoveTrDOTS : MoveAbstract
{
    
    protected override void Config()
    {
        throw new System.NotImplementedException();
    }
    /*
    void MyStart()
    {
        MoveSystem moveSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<MoveSystem>();
        moveSystem.OnIdle += MoveSystem_OnIdle;
    }

    private void MoveSystem_OnIdle(object sender, System.EventArgs e)
    {
        OnIdle();
    }

    public static bool MyStaticFixedUpdate(ref MovementAspect moveAspect) //Recibir un aspecto con todas las variables
    {
        //localTr.Position += ((float3)(*moveSpeed.speed)) * deltaTime;

        //moveAspect.localTransform.ValueRW.Position += ((float3) *(moveAspect.moveSpeed.ValueRO.speed)* Time.fixedDeltaTime);

        var VelocityCalculate = moveAspect.moveSpeed.ValueRO.speed;

        VelocityCalculate -= moveAspect.desaceleration * Time.fixedDeltaTime * moveAspect.moveSpeed.ValueRO.speed.normalized;

        if (VelocityCalculate.sqrMagnitude <= 0)
            return true;
        else
            return false;
    }
    */
}

/*
public readonly partial struct MovementAspect : IAspect
{
    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRW<MoveSpeedComponent> moveSpeed;
    public readonly float desaceleration;
}
*/