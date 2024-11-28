using Unity.Burst;
using Unity.Entities;

partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach ((RefRO<Hunger> hunger, RefRW<Health> health, Entity entity) in SystemAPI.Query<RefRO<Hunger>, RefRW<Health>>().WithEntityAccess())
        {
            if (health.ValueRO.Value <= 0)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            health.ValueRW.Timer += SystemAPI.Time.DeltaTime;
            if (health.ValueRO.Timer < health.ValueRO.TimerMax) continue;
            health.ValueRW.Timer = 0f;

            int valueToChange = health.ValueRO.HungerThreshold <= hunger.ValueRO.Value ? 1 : -1;

            health.ValueRW.Value += valueToChange;

            if (health.ValueRO.Value > health.ValueRO.MaxHealth)
            {
                health.ValueRW.Value = health.ValueRO.MaxHealth;
            }
        }
    }
}
