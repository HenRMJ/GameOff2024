using Unity.Entities;
using UnityEngine;

public class FoundryAuthoring : MonoBehaviour
{
    [SerializeField] private int Wood;
    [SerializeField] private int Stone;
    
    public class Baker : Baker<FoundryAuthoring>
    {
        public override void Bake(FoundryAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Foundry
            {
                Wood = authoring.Wood,
                Stone = authoring.Stone
            });
        }
    }
}

public struct Foundry : IComponentData
{
    public int Wood;
    public int Stone;
}