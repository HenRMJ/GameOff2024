using Unity.Entities;
using UnityEngine;

public class ResourceContainerAuthoring : MonoBehaviour
{
    [SerializeField] private int resourcesAvailable;
    [SerializeField] private int resistanceToCollection;
    [SerializeField] private CultistTypes typeToGiveBoost;
    
    public class Baker : Baker<ResourceContainerAuthoring>
    {
        public override void Bake(ResourceContainerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceContainer
            {
                AvailableResources = authoring.resourcesAvailable,
                Resistance = authoring.resistanceToCollection,
                TypeToGiveBoost = authoring.typeToGiveBoost
            });
        }
    }
}

public struct ResourceContainer : IComponentData
{
    public CultistTypes TypeToGiveBoost;
    public bool MarkedForBoost;
    public int AvailableResources;
    public int Resistance;
    public int ResistanceAccumulator;
}