using Unity.Entities;
using UnityEngine;

public class HappinessAuthoring : MonoBehaviour
{
    [SerializeField] private float _maxTimer;
    public class Baker : Baker<HappinessAuthoring>
    {
        public override void Bake(HappinessAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Happiness
            {
                maxTimer = authoring._maxTimer
            });
        }
    }
}

public struct Happiness : IComponentData
{
    public float happiness;
    public float timer;
    public float maxTimer;
}