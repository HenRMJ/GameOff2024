using Unity.Entities;
using UnityEngine;

public class ProductivityAuthoring : MonoBehaviour
{
    [SerializeField] private float contributationInterval;
    public class Baker : Baker<ProductivityAuthoring>
    {
        public override void Bake(ProductivityAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Productivity
            {
                ContributationInterval = authoring.contributationInterval
            });
        }
    }
}

public struct Productivity : IComponentData
{
    public Entity ContributationEntity;
    public float ContributationInterval;
    public float ContributionTimer;
}