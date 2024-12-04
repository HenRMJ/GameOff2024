using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct FertilitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;

        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

        CollisionFilter collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1 << 6,
            GroupIndex = 0
        };
            
        foreach ((RefRW<Fertility> fertility, RefRO<LocalTransform> localTransform) in SystemAPI.Query<RefRW<Fertility>, RefRO<LocalTransform>>())
        {
            fertility.ValueRW.ArouseTimer += SystemAPI.Time.DeltaTime;
            if (fertility.ValueRO.ArouseTimer < fertility.ValueRO.ArouseInterval) continue;
            fertility.ValueRW.ArouseTimer = 0f;

            collisionWorld.OverlapSphere(localTransform.ValueRO.Position, fertility.ValueRO.Range, ref distanceHits,
                collisionFilter);

            foreach (DistanceHit distanceHit in distanceHits)
            {
                if (SystemAPI.Exists(distanceHit.Entity) &&
                    SystemAPI.HasComponent<Arousal>(distanceHit.Entity))
                {
                    RefRW<Arousal> arousal = SystemAPI.GetComponentRW<Arousal>(distanceHit.Entity);

                    arousal.ValueRW.ArousalValue++;
                }
            }
        }
        
        foreach (var (cleanupNameBlob, entity) in SystemAPI.Query <RefRW<CleanupNameBlob>>().WithEntityAccess())
        {
            if (state.EntityManager.HasComponent<Disabled>(entity))
            {
                if (cleanupNameBlob.ValueRO.NamesBlob.IsCreated)
                {
                    cleanupNameBlob.ValueRW.NamesBlob.Dispose();
                }
            }

            state.EntityManager.RemoveComponent<CleanupNameBlob>(entity);
        }
    }
}
