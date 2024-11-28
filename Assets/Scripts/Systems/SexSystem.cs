using Rukhanka;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateAfter(typeof(FeedingSystem))]
partial struct SexSystem : ISystem
{
    private FastAnimatorParameter _fuckingParam;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<School>();
        state.RequireForUpdate<PhysicsWorldSingleton>();

        _fuckingParam = new FastAnimatorParameter("fucking");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
        CollisionFilter filter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << 6,
            GroupIndex = 0
        };
        
        School school = SystemAPI.GetSingleton<School>();
        
        foreach ((AnimatorParametersAspect animator, RefRW<TargetOverride> targetOverride, RefRO<LocalTransform> localTransform, RefRO<Happiness> happiness, RefRW<Arousal> arousal, Entity entity) 
                 in SystemAPI.Query<AnimatorParametersAspect, RefRW<TargetOverride>, RefRO<LocalTransform>, RefRO<Happiness>, RefRW<Arousal>>().WithEntityAccess())
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
                    animator.SetParameterValue(_fuckingParam, true);

                    arousal.ValueRW.FuckingTimer += SystemAPI.Time.DeltaTime;
                    if (arousal.ValueRO.FuckingTimer <= arousal.ValueRO.TimeToFuck) continue;
                    arousal.ValueRW.FuckingTimer = 0f;
                    
                    Arousal partnerArousal = SystemAPI.GetComponent<Arousal>(arousal.ValueRW.Partner);

                    animator.SetParameterValue(_fuckingParam, false);
                    
                    if (partnerArousal.ArousalValue != 0)
                    {
                        school.Children++;
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
            
            if (happiness.ValueRO.Value >= arousal.ValueRO.HappinessThreshold)
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
                            filter))
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
                    animator.SetParameterValue(_fuckingParam, true);
                    
                    arousal.ValueRW.FuckingTimer += SystemAPI.Time.DeltaTime;
                    if (arousal.ValueRO.FuckingTimer <= arousal.ValueRO.TimeToFuck) continue;
                    arousal.ValueRW.FuckingTimer = 0f;
                    
                    animator.SetParameterValue(_fuckingParam, false);
                    
                    if (setPartnerArousal.ArousalValue != 0)
                    {
                        school.Children++;
                    }
                    
                    targetOverride.ValueRW.TargetEntity = Entity.Null;
                    arousal.ValueRW.Partner = Entity.Null;
                    arousal.ValueRW.ArousalValue = 0;
                }
            }
        }

        SystemAPI.SetSingleton(school);
    }
}
