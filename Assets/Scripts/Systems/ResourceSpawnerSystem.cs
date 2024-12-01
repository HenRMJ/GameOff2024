using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

partial struct ResourceSpawnerSystem : ISystem
{
    private CollisionFilter _collisionFilter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        _collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = (1 << 6) | (1 << 7),
            GroupIndex = 0
        };
    }
    
    public void OnUpdate(ref SystemState state)
    {
        float DeltaTime = SystemAPI.Time.DeltaTime;

        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
        
        foreach ((RefRW<ResourceSpawner> spawner, Entity entity) in SystemAPI.Query<RefRW<ResourceSpawner>>().WithEntityAccess())
        {
            if (!spawner.ValueRO.Initialized)
            {
                spawner.ValueRW.Initialized = true;
                spawner.ValueRW.Random = new Random((uint)entity.Index);
                spawner.ValueRW.Random.NextFloat();
            }
            
            spawner.ValueRW.SpawnTimer += DeltaTime;
            if (spawner.ValueRO.SpawnTimer < spawner.ValueRO.SpawnInterval) continue;
            spawner.ValueRW.SpawnTimer = 0f;

            float3 spawnPosition = new float3(
                spawner.ValueRW.Random.NextFloat(spawner.ValueRO.SpawnAreaMin.x, spawner.ValueRO.SpawnAreaMax.x),
                0f,
                spawner.ValueRW.Random.NextFloat(spawner.ValueRO.SpawnAreaMin.y, spawner.ValueRO.SpawnAreaMax.y)
            );

            distanceHits.Clear();
            if (!collisionWorld.OverlapSphere(spawnPosition, spawner.ValueRO.ClearanceRange, ref distanceHits,
                    _collisionFilter))
            {
                Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.ResourceEntity);
                LocalTransform transform = state.EntityManager.GetComponentData<LocalTransform>(spawnedEntity);
                ResourceContainer container = state.EntityManager.GetComponentData<ResourceContainer>(spawnedEntity);
                
                transform.Position = spawnPosition;
                container.Resistance += spawner.ValueRW.Random.NextInt(-10, 10);
                container.AvailableResources += spawner.ValueRW.Random.NextInt(-5, 5);
                
                state.EntityManager.SetComponentData(spawnedEntity, transform);
                state.EntityManager.SetComponentData(spawnedEntity, container);
                RescanScene.Rescan();
            }
        }
    }
}
