using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoverAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float rotationalSpeed;
    
    public class Baker : Baker<MoverAuthoring>
    {
        public override void Bake(MoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Mover
            {
                MoveSpeed = authoring.moveSpeed,
                RotationalSpeed = authoring.rotationalSpeed
            });
        }
    }
}

public struct Mover : IComponentData
{
    public float3 TargetPosition;
    public float RotationalSpeed;
    public float MoveSpeed;
    public bool IsMoving;
}