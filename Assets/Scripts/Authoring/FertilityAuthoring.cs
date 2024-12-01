using Unity.Entities;
using UnityEngine;

public class FertilityAuthoring : MonoBehaviour
{
    [SerializeField] private float arouseInterval;
    [SerializeField] private float range;
    
    public class Baker : Baker<FertilityAuthoring>
    {
        public override void Bake(FertilityAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Fertility
            {
                ArouseInterval = authoring.arouseInterval,
                Range = authoring.range
            });
        }
    }
}

public struct Fertility : IComponentData
{
    public float ArouseInterval;
    public float ArouseTimer;
    public float Range;
}
