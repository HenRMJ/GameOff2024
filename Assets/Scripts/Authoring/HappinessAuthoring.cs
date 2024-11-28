using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class HappinessAuthoring : MonoBehaviour
{
    [SerializeField] private float maxTimer;
    [SerializeField] private int hungerThreshold;
    public class Baker : Baker<HappinessAuthoring>
    {
        public override void Bake(HappinessAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Happiness
            {
                MaxTimer = authoring.maxTimer,
                HungerThreshold = authoring.hungerThreshold
            });
        }
    }
}

public struct Happiness : IComponentData
{
    public float Value;
    public int HungerThreshold;
    public float Timer;
    public float MaxTimer;
}