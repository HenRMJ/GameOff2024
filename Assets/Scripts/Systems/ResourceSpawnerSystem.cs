using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

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
        
        foreach (RefRW<ResourceSpawner> spawner in SystemAPI.Query<RefRW<ResourceSpawner>>())
        {
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

                transform.Position = spawnPosition;
                
                state.EntityManager.SetComponentData(spawnedEntity, transform);
                RescanScene.Rescan();
            }
        }
    }
}
