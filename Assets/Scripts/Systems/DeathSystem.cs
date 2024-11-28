using Unity.Burst;
using Unity.Entities;

partial struct DeathSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        new DeathJob
        {
            Buffer = entityCommandBuffer.AsParallelWriter()
        }.ScheduleParallel(state.Dependency).Complete();
    }
}

[BurstCompile]
public partial struct DeathJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Buffer;

    public void Execute(in Age age, in Health health, [EntityIndexInQuery] int jobIndex, Entity entity)
    {
        if (age.Value >= 100 ||
            health.Value <= 0)
        {
            Buffer.DestroyEntity(jobIndex, entity);
        }
    }
}
