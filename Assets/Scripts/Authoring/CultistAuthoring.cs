using Unity.Entities;
using UnityEngine;

public class CultistAuthoring : MonoBehaviour
{
    [SerializeField] private int _age;
    [SerializeField] private float _hunger;
    [SerializeField] private float _health;
    [SerializeField] private float _devotion;
    [SerializeField] private float _happiness;
    
    public class Baker : Baker<CultistAuthoring>
    {
        public override void Bake(CultistAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Cultist
            {
                Age = authoring._age,
                Hunger = authoring._hunger,
                Health = authoring._health,
                Devotion = authoring._devotion,
                Happiness = authoring._happiness
            });
        }
    }
}

public struct Cultist : IComponentData
{
    public int Age;
    public float Hunger;
    public float Health;
    public float Devotion;
    public float Happiness;
}