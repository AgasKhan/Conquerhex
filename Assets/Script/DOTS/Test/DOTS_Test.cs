using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using CheatCommandsPrompt;

unsafe public class DOTS_Test : MonoBehaviour
{
    static DOTS_Test instance;

    public Vector3 _velocityCalculate;

    public Vector3 VelocityCalculate
    {
        get
        {
            return _velocityCalculate;
        }

        set
        {
            _velocityCalculate = value;
        }
    }

    [Command]
    static void Calculate(float x, float y, float z)
    {
        instance.VelocityCalculate = new Vector3(x, y, z);
    }

    private void Awake()
    {
        instance = this;
    }

    public class Baker : Baker<DOTS_Test>
    {
        unsafe public override void Bake(DOTS_Test authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            //AddComponent(entity, new MoveSpeedComponent());

            //authoring.componentSpeed = new MoveSpeedComponent()
            
            fixed (Vector3* pSpeed = &authoring._velocityCalculate)
            {
                AddComponent(entity, new MoveSpeedComponent() { speed = pSpeed });
            }
            
        }
    }

}

[ChunkSerializable]
unsafe public struct MoveSpeedComponent : IComponentData
{
    public Vector3* speed;
}
