using Unity.Entities;
using UnityEngine;

public class KitchenAuthoring : MonoBehaviour
{
    [SerializeField] private int _water;
    [SerializeField] private int _meat;
    [SerializeField] private int _vegetables;
    
    public class Baker : Baker<KitchenAuthoring>
    {
        public override void Bake(KitchenAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Kitchen
            {
                Water = authoring._water,
                Meat = authoring._meat,
                Vegetables = authoring._vegetables
            });
        }
    }
}

public struct Kitchen : IComponentData
{
    public int Water;
    public int Meat;
    public int Vegetables;
}