using Unity.Entities;
using UnityEngine;

public class HungerAuthoring : MonoBehaviour
{
    [SerializeField] private float timerMax;
    [SerializeField] private int startingHunger;
    [SerializeField] private int maxHunger;
    
    public class Baker : Baker<HungerAuthoring>
    {
        public override void Bake(HungerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Hunger
            {
                TimerMax = authoring.timerMax,
                Value = authoring.startingHunger,
                MaxHunger = authoring.maxHunger
            });
        }
    }
}

public struct Hunger : IComponentData
{
    public int Value;
    public int MaxHunger;
    public float Timer;
    public float TimerMax;
}