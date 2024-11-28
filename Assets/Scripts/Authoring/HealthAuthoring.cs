using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    [SerializeField] private int startingHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private float timerMax;
    [SerializeField] private int hungerThreshold;
    
    public class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health
            {
                Value = authoring.startingHealth,
                MaxHealth = authoring.maxHealth,
                TimerMax = authoring.timerMax,
                HungerThreshold = authoring.hungerThreshold
            });
        }
    }
}

public struct Health : IComponentData
{
    public int Value;
    public int MaxHealth;
    public int HungerThreshold;
    public float Timer;
    public float TimerMax;
}