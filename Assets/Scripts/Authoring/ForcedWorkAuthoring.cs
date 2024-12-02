using Unity.Entities;
using UnityEngine;

public class ForcedWorkAuthoring : MonoBehaviour
{
    public class Baker : Baker<ForcedWorkAuthoring>
    {
        public override void Bake(ForcedWorkAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new ForcedWork());
        }
    }
}

public struct ForcedWork : IComponentData
{
    public bool Forced;
    public CultResources ResourceToSearchFor;
}