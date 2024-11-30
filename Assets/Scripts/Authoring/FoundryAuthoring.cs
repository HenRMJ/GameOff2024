using Unity.Entities;
using UnityEngine;

public class FoundryAuthoring : MonoBehaviour
{
    public class Baker : Baker<FoundryAuthoring>
    {
        public override void Bake(FoundryAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Foundry());
        }
    }
}

public struct Foundry : IComponentData
{
    public int Wood;
    public int Stone;
}