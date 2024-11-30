using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CultistTypeAuthoring : MonoBehaviour
{
    [SerializeField] private CultistTypes type;

    public class Baker : Baker<CultistTypeAuthoring>
    {
        public override void Bake(CultistTypeAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new CultistType
            {
                Type = authoring.type
            });
        }
    }
}

public struct CultistType : IComponentData
{
    public CultistTypes Type;
}
