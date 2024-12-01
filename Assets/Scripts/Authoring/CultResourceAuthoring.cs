 using Unity.Entities;
using UnityEngine;

public class CultResourceAuthoring : MonoBehaviour
{
    [SerializeField] private CultResources resource;
    
    public class Baker : Baker<CultResourceAuthoring>
    {
        public override void Bake(CultResourceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CultResource
            {
                Resource = authoring.resource
            });
        }
    }
}

public struct CultResource : IComponentData
{
    public CultResources Resource;
}
