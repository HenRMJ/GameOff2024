using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AgeAuthoring : MonoBehaviour
{
    [SerializeField] private float TimerToAge;
    
    public class Baker : Baker<AgeAuthoring>
    {
        public override void Bake(AgeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Age
            {
                Value = 18,
                TimerToAge = authoring.TimerToAge
            });
        }
    }
}

public struct Age : IComponentData
{
    public float Timer;
    public float TimerToAge;
    public int Value;
}
