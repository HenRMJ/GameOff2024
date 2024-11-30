using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class RandomGoatWalkingAuthoring : MonoBehaviour
{
    [SerializeField] private float waitInterval;
    [SerializeField] private float moveRange;
    
    public class Baker : Baker<RandomGoatWalkingAuthoring>
    {
        public override void Bake(RandomGoatWalkingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomGoatWalking
            {
                Random = new Random(),
                WaitInterval = authoring.waitInterval,
                RangeToMove = authoring.moveRange
            });
        }
    }
}

public struct RandomGoatWalking : IComponentData
{
    public bool Initialized;
    public Random Random;

    public float WaitInterval;
    public float WaitTimer;
    public float RangeToMove;
}
