using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTr : MoveAbstract
{
    SpriteRenderer rend;
    
    protected override void Config()
    {
        //MyAwakes += MyAwake;
        MyFixedUpdates += MyFixedUpdate;

        /*
        if (rend != null)
            MyUpdates += MyUpdateRender;
        */
    }

    void MyAwake()
    {
        rend = GetComponentInChildren<SpriteRenderer>();

        /*
        if (rend != null)
            MyUpdates += MyUpdateRender;
        */
    }

    protected virtual void MyFixedUpdate()
    {
        transform.position += (VelocityCalculate * Time.fixedDeltaTime);

        VelocityCalculate -= _desaceleration.current * Time.fixedDeltaTime * VelocityCalculate.normalized;

        if (VelocityCalculate.sqrMagnitude <= 0)
            OnIdle();
        else
            OnMove(VelocityCalculate);
    }

    public static void MyStaticFixedUpdate(ref Unity.Transforms.LocalTransform localTr) //Recibir un aspecto con todas las variables
    {
        //localTr.Position += ((float3)(*moveSpeed.speed)) * deltaTime;
    }


    void MyUpdateRender()
    {
        rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        //rend.flipX = ! (Vector2.Dot(vectorVelocity, Vector2.right) > 0.5) && ;
            
    }

}
