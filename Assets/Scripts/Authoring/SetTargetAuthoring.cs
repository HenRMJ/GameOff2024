using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SetTargetAuthoring : MonoBehaviour
{
    public class Baker : Baker<SetTargetAuthoring>
    {
        public override void Bake(SetTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SetTarget
            {
                TargetPosition = authoring.transform.position,
            });
        }
    }
}

public struct SetTarget : IComponentData
{
    public float3 TargetPosition;
}