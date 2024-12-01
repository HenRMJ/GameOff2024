using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ResourceSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float clearanceRange;
    [SerializeField] private Vector2 bottomLeftCornerPosition;
    [SerializeField] private Vector2 topRightCornerPosition;
    
    public class Baker : Baker<ResourceSpawnerAuthoring>
    {
        public override void Bake(ResourceSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceSpawner
            {
                ResourceEntity = GetEntity(authoring.resourcePrefab, TransformUsageFlags.None),
                Random = new Random((uint)entity.Index),
                SpawnInterval = authoring.spawnInterval,
                SpawnAreaMin = authoring.bottomLeftCornerPosition,
                SpawnAreaMax = authoring.topRightCornerPosition,
                ClearanceRange = authoring.clearanceRange
            });
        }
    }
}

public struct ResourceSpawner : IComponentData
{
    public bool Initialized;
    public Entity ResourceEntity;
    public float SpawnInterval;
    public float SpawnTimer;
    public float ClearanceRange;
    public float2 SpawnAreaMin;
    public float2 SpawnAreaMax;
    public Random Random;
}