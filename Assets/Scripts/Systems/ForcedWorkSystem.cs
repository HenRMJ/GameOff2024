using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ForcedWorkSystem : ISystem
{
    private ComponentLookup<CultResource> _resources;
    private ComponentLookup<LocalTransform> _resourceTransforms;
    private EntityQuery _query;
    
    public void OnCreate(ref SystemState state)
    {
        _resources = state.GetComponentLookup<CultResource>(true);
        _resourceTransforms = state.GetComponentLookup<LocalTransform>(true);
        _query = state.GetEntityQuery(
            ComponentType.ReadOnly<CultResource>(), 
            ComponentType.ReadOnly<ResourceContainer>(),
            ComponentType.ReadOnly<LocalTransform>());
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> entities = _query.ToEntityArray(Allocator.TempJob);
        
        _resources.Update(ref state);
        _resourceTransforms.Update(ref state);

        new FindClosestEntityJob
        {
            Entities = entities,
            CultResourceLookup = _resources,
            CultLocalTransforms = _resourceTransforms
        }.ScheduleParallel(state.Dependency).Complete();
        
        entities.Dispose();
    }
}

[BurstCompile]
public partial struct FindClosestEntityJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public NativeArray<Entity> Entities;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<CultResource> CultResourceLookup;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<LocalTransform> CultLocalTransforms;
    
    public void Execute(in ForcedWork forced,
        ref SetTarget target,
        ref CultResource resource,
        ref Productivity productivity,
        in LocalTransform localTransform)
    {
        if (!forced.Forced) return;
            
        float3 closestPosition = float3.zero;
        Entity closestEntity = Entity.Null;
        float closestDistance = float.MaxValue;
            
        foreach (Entity entity in Entities)
        {
            if (CultResourceLookup[entity].Resource != forced.ResourceToSearchFor) continue;

            float3 entityPosition = CultLocalTransforms[entity].Position;
            float distance = math.distance(localTransform.Position, entityPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = entityPosition;
                closestEntity = entity;
            }
        }

        if (closestEntity == Entity.Null) return;

        if (!math.all(target.TargetPosition == closestPosition))
        {
            target.TargetPosition = closestPosition;
        }
            
        productivity.ContributationEntity = closestEntity;
        resource.Resource = forced.ResourceToSearchFor;
    }
}
