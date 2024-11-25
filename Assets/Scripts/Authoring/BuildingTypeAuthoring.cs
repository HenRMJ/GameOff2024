using Unity.Entities;
using UnityEngine;

public class BuildingTypeAuthoring : MonoBehaviour
{
    [SerializeField] private BuildingTypes _buildingTypes;
    
    public class Baker : Baker<BuildingTypeAuthoring>
    {
        public override void Bake(BuildingTypeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingType
            {
                Building = authoring._buildingTypes
            });
        }
    }
}

public struct BuildingType : IComponentData
{
    public BuildingTypes Building;
}