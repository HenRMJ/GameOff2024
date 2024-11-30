using Unity.Entities;
using UnityEngine;

public class ResourceContainerAuthoring : MonoBehaviour
{
    [SerializeField] private int resourcesAvailable;
    [SerializeField] private int resistanceToCollection;
    
    public class Baker : Baker<ResourceContainerAuthoring>
    {
        public override void Bake(ResourceContainerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceContainer
            {
                AvailableResources = authoring.resourcesAvailable,
                Resistance = authoring.resistanceToCollection
            });
        }
    }
}

public struct ResourceContainer : IComponentData
{
    public int AvailableResources;
    public int Resistance;
}