using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

unsafe public class DOTS_Test : MonoBehaviour
{
    [SerializeField] private GameObject cube;

    public MoveSpeedComponent* componentSpeed;

    private void Start()
    {
        
    }

    
    public class Baker : Baker<DOTS_Test>
    {
        unsafe public override void Bake(DOTS_Test authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);


            AddComponent(entity, new MoveSpeedComponent());

            //authoring.componentSpeed = new MoveSpeedComponent()
            
            
            /*
            fixed (Vector3* pSpeed = &authoring.speed)
            {
                AddComponent(entity, new MoveSpeedComponent() { speed = pSpeed });
            }
            */
        }
    }

}

[ChunkSerializable]
unsafe public struct MoveSpeedComponent : IComponentData
{
    public Vector3 speed;
}
