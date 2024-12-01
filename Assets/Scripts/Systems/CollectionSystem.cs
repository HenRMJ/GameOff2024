using Rukhanka;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct CollectionSystem : ISystem
{
    private CollisionFilter _collisionFilter;
    private FastAnimatorParameter _chopTrigger;
    private FastAnimatorParameter _hitTrigger;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Kitchen>();
        state.RequireForUpdate<Foundry>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        _collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << 7,
            GroupIndex = 0
        };
        _chopTrigger = new FastAnimatorParameter("chop");
        _hitTrigger = new FastAnimatorParameter("hit");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

        
        Foundry foundry = SystemAPI.GetSingleton<Foundry>();
        Kitchen kitchen = SystemAPI.GetSingleton<Kitchen>();

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var (type, productivity, cultist, cultResource, localTransform, entity) in SystemAPI
                     .Query<RefRO<CultistType>, RefRW<Productivity>, RefRO<Cultist>, RefRW<CultResource>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            if (cultResource.ValueRO.Resource == CultResources.None) continue;
            
            productivity.ValueRW.ContributionTimer += deltaTime;
            if (productivity.ValueRO.ContributionTimer < productivity.ValueRO.ContributationInterval) continue;
            productivity.ValueRW.ContributionTimer = 0f;

            if (!SystemAPI.Exists(productivity.ValueRO.ContributationEntity) ||
                !SystemAPI.HasComponent<ResourceContainer>(productivity.ValueRO.ContributationEntity))
            {
                productivity.ValueRW.ContributationEntity = Entity.Null;
                cultResource.ValueRW.Resource = CultResources.None;
                continue;
            }

            distanceHits.Clear();

            bool withinDistance = false;
            
            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, 3f, ref distanceHits, _collisionFilter))
            {
                foreach (DistanceHit hit in distanceHits)
                {
                    if (hit.Entity == productivity.ValueRO.ContributationEntity)
                    {
                        withinDistance = true;
                    }
                }
            }

            if (!withinDistance) continue;
            
            RefRW<ResourceContainer> resourceContainer =
                SystemAPI.GetComponentRW<ResourceContainer>(productivity.ValueRO.ContributationEntity);
            
            AnimatorParametersAspect animator = SystemAPI.GetAspect<AnimatorParametersAspect>(entity);
            animator.SetTrigger(_chopTrigger);

            if (resourceContainer.ValueRO.TypeToGiveBoost == type.ValueRO.Type)
            {
                resourceContainer.ValueRW.MarkedForBoost = true;
            }
            
            resourceContainer.ValueRW.ResistanceAccumulator += cultist.ValueRO.Level;
            if (resourceContainer.ValueRO.ResistanceAccumulator < resourceContainer.ValueRO.Resistance) continue;
            resourceContainer.ValueRW.ResistanceAccumulator -= resourceContainer.ValueRO.Resistance;

            resourceContainer.ValueRW.AvailableResources--;
            
            switch (cultResource.ValueRO.Resource)
            {
                case CultResources.Wood:
                    foundry.Wood += resourceContainer.ValueRO.MarkedForBoost ? 2 : 1;
                    break;
                case CultResources.Rock:
                    foundry.Stone += resourceContainer.ValueRO.MarkedForBoost ? 2 : 1;
                    break;
                case CultResources.Meat:
                    kitchen.Meat += resourceContainer.ValueRO.MarkedForBoost ? 2 : 1;

                    if (SystemAPI.Exists(productivity.ValueRO.ContributationEntity))
                    {
                        SystemAPI.GetAspect<AnimatorParametersAspect>(productivity.ValueRO.ContributationEntity).SetTrigger(_hitTrigger);
                    }
                    break;
                case CultResources.Vegetables:
                    kitchen.Vegetables += resourceContainer.ValueRO.MarkedForBoost ? 2 : 1;
                    break;
                case CultResources.Water:
                    kitchen.Water += resourceContainer.ValueRO.MarkedForBoost ? 2 : 1;
                    break;
            }

            SystemAPI.SetSingleton(kitchen);
            SystemAPI.SetSingleton(foundry);
            
            resourceContainer.ValueRW.MarkedForBoost = false;
            
            if (resourceContainer.ValueRO.AvailableResources <= 0)
            {
                if (SystemAPI.Exists(productivity.ValueRO.ContributationEntity))
                {
                    entityCommandBuffer.DestroyEntity(productivity.ValueRO.ContributationEntity);
                }
                else
                {
                    productivity.ValueRW.ContributationEntity = Entity.Null;
                    cultResource.ValueRW.Resource = CultResources.None;
                }
            }
        } 
    }
}
