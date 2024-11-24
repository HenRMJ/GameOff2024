using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TargetOverrideAuthoring : MonoBehaviour
{
    public class Baker : Baker<TargetOverrideAuthoring>
    {
        public override void Bake(TargetOverrideAuthoring authoring)
        {
            AddComponent<TargetOverride>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}

public struct TargetOverride : IComponentData
{
    public Entity TargetEntity;
}