using Unity.Entities;
using UnityEngine;

public class AvatarAuthoring : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float devotionTimer;
    
    public class Baker : Baker<AvatarAuthoring>
    {
        public override void Bake(AvatarAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Avatar
            {
                Range = authoring.range,
                DevotionTimer = authoring.devotionTimer
            });
        }
    }
}

public struct Avatar : IComponentData
{
    public int Devotion;
    public int BaseContributionCurve;
    public float Range;
    public float Timer;
    public float DevotionTimer;
}
