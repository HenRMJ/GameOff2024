using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateAfter(typeof(FeedingSystem))]
partial struct SexSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<PhysicsWorldSingleton>();

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
        CollisionFilter collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << 6,
            GroupIndex = 0
        };
        
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach ((RefRW<TargetOverride> targetOverride, RefRO<LocalTransform> localTransform, RefRO<Happiness> happiness, RefRW<Arousal> arousal, Entity entity) 
                 in SystemAPI.Query<RefRW<TargetOverride>, RefRO<LocalTransform>, RefRO<Happiness>, RefRW<Arousal>>().WithEntityAccess())
        {
            if (targetOverride.ValueRO.TargetEntity != Entity.Null)
            {
                if (arousal.ValueRO.Partner == Entity.Null) continue;

                LocalTransform partnerTransform = SystemAPI.GetComponent<LocalTransform>(arousal.ValueRO.Partner);
                float distanceSq = math.distance(localTransform.ValueRO.Position,
                    partnerTransform.Position);
                
                if (distanceSq <=
                    arousal.ValueRO.ReproductionRangeSq)
                {
                    Arousal partnerArousal = SystemAPI.GetComponent<Arousal>(arousal.ValueRW.Partner);

                    if (partnerArousal.ArousalValue != 0)
                    {
                        entityCommandBuffer.Instantiate(entitiesReferences.CultistEntity);
                    }
                    
                    targetOverride.ValueRW.TargetEntity = Entity.Null;
                    arousal.ValueRW.Partner = Entity.Null;
                    arousal.ValueRW.ArousalValue = 0;
                }

                continue;
            }
            
            arousal.ValueRW.Timer += SystemAPI.Time.DeltaTime;
            if (arousal.ValueRO.Timer < arousal.ValueRO.CheckTimer) continue;
            arousal.ValueRW.Timer = 0f;
            
            if (happiness.ValueRO.happiness >= arousal.ValueRO.HappinessThreshold)
            {
                arousal.ValueRW.ArousalValue++;
            }

            if (arousal.ValueRO.ArousalValue >= arousal.ValueRO.SeekValue)
            {
                if (arousal.ValueRO.Partner == Entity.Null)
                {
                    distanceHits.Clear();

                    if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position,
                            arousal.ValueRO.SeekDistance,
                            ref distanceHits,
                            collisionFilter))
                    {
                        Entity temporaryPartner = Entity.Null;
                        
                        foreach (DistanceHit hit in distanceHits)
                        {
                            if (hit.Entity == entity) continue;
                            if (!SystemAPI.Exists(hit.Entity) ||
                                !SystemAPI.HasComponent<Arousal>(hit.Entity)) continue;

                            Arousal partnerArousal = SystemAPI.GetComponent<Arousal>(hit.Entity);

                            if (partnerArousal.Partner == entity)
                            {
                                arousal.ValueRW.Partner = hit.Entity;
                                targetOverride.ValueRW.TargetEntity = hit.Entity;
                                break;
                            }

                            if (partnerArousal.Partner != Entity.Null) continue;
                            if (temporaryPartner != Entity.Null) continue;
                            
                            if (partnerArousal.ArousalValue >= partnerArousal.SeekValue)
                            {
                                temporaryPartner = hit.Entity;
                            }
                        }

                        if (temporaryPartner != Entity.Null &&
                            arousal.ValueRO.Partner == Entity.Null)
                        {
                            arousal.ValueRW.Partner = temporaryPartner;
                            targetOverride.ValueRW.TargetEntity = temporaryPartner;
                        }
                    }
                }

                if (arousal.ValueRO.Partner == Entity.Null) continue;

                float distanceSq = math.distance(localTransform.ValueRO.Position,
                    SystemAPI.GetComponent<LocalTransform>(arousal.ValueRO.Partner).Position);
                
                Arousal setPartnerArousal = SystemAPI.GetComponent<Arousal>(arousal.ValueRW.Partner);
                TargetOverride setPartnerTargetOverride =
                    SystemAPI.GetComponent<TargetOverride>(arousal.ValueRO.Partner);
                
                setPartnerArousal.Partner = entity;
                setPartnerTargetOverride.TargetEntity = entity;
                
                SystemAPI.SetComponent(arousal.ValueRO.Partner, setPartnerArousal);
                SystemAPI.SetComponent(arousal.ValueRO.Partner, setPartnerTargetOverride);

                
                if (distanceSq <=
                    arousal.ValueRO.ReproductionRangeSq)
                {
                    if (setPartnerArousal.ArousalValue != 0)
                    {
                        Entity childEntity = entityCommandBuffer.Instantiate(entitiesReferences.CultistEntity);

                        Random random = entitiesReferences.Random;

                        random.state += random.state + ((uint)SystemAPI.Time.ElapsedTime * 1000);
                        ref NameBlob names = ref entitiesReferences.NamesBlob.Value;
                        
                        entityCommandBuffer.SetComponent(childEntity, new Name
                        {
                            FirstName = names.FirstNames[random.NextInt(0, names.FirstNames.Length)],
                            LastName = names.LastNames[random.NextInt(0, names.LastNames.Length)]
                        });

                        entitiesReferences.Random = random;
                        SystemAPI.SetSingleton(entitiesReferences);
                    }
                    
                    targetOverride.ValueRW.TargetEntity = Entity.Null;
                    arousal.ValueRW.Partner = Entity.Null;
                    arousal.ValueRW.ArousalValue = 0;

                }
            }
        }
    }
}
