using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class NameAuthoring : MonoBehaviour
{
    public class Baker : Baker<NameAuthoring>
    {
        public override void Bake(NameAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Name
            {
                FirstName = NameDatabase.GetRandonFirstName(),
                LastName = NameDatabase.GetRandomLastName()
            });
        }
    }
}

public struct Name : IComponentData
{
    public FixedString32Bytes FirstName;
    public FixedString32Bytes LastName;
}
