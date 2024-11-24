using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup)), UpdateBefore(typeof(ResetEventsSystem))]
partial struct SelectedVisualSystem : ISystem
{
    private NativeQueue<VisualChange> _visualChange;
    private ComponentLookup<LocalTransform> localTransformLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _visualChange = new NativeQueue<VisualChange>(Allocator.Persistent);
        localTransformLookup = state.GetComponentLookup<LocalTransform>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SelectedVisualJob visualJob = new SelectedVisualJob
        {
            VisualChanges = _visualChange.AsParallelWriter()
        };
        JobHandle handled = visualJob.ScheduleParallel(state.Dependency);
        
        handled.Complete();

        localTransformLookup.Update(ref state);
        
        while (_visualChange.TryDequeue(out VisualChange change))
        {
            LocalTransform localTransform = localTransformLookup[change.Entity];
            localTransform.Scale = change.Scale;
            localTransformLookup[change.Entity] = localTransform;
        }
        
        localTransformLookup.Update(ref state);
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        _visualChange.Dispose();
    }
}

[BurstCompile, WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct SelectedVisualJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public NativeQueue<VisualChange>.ParallelWriter VisualChanges;
    
    public void Execute(ref Selected selected)
    {
        VisualChange change = new VisualChange
        {
            Entity = Entity.Null
        };
        
        if (selected.onDeselected)
        {
            change.Entity = selected.visualEntity;
            change.Scale = 0f;
        }
        
        if (selected.onSelected)
        {
            change.Entity = selected.visualEntity;
            change.Scale = selected.showScale;
        }
        
        if (change.Entity != Entity.Null)
        {
            VisualChanges.Enqueue(change);
        }
    }
}

public struct VisualChange
{
    public Entity Entity;
    public float Scale;
}