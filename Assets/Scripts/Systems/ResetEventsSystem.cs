using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{
    private NativeArray<JobHandle> jobHandleNativeArray;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        jobHandleNativeArray = new NativeArray<JobHandle>(1, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ResetSelectedEventsJob resetSelected = new ResetSelectedEventsJob();
        resetSelected.ScheduleParallel();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        jobHandleNativeArray.Dispose();
    }
}

[BurstCompile, WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct ResetSelectedEventsJob : IJobEntity
{
    public void Execute(ref Selected selected)
    {
        selected.onSelected = false;
        selected.onDeselected = false;
    }
} 