using Unity.Burst;
using Unity.Entities;

[UpdateAfter(typeof(HungerSystem))]
partial struct HappinessSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new CheckHappinessJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct CheckHappinessJob : IJobEntity
{
    public float DeltaTime;
    
    public void Execute(ref Happiness happiness, in Hunger hunger)
    {
        happiness.Timer += DeltaTime;
        if (happiness.Timer <= happiness.MaxTimer) return;
        happiness.Timer = 0f;

        if (hunger.Value >= happiness.HungerThreshold)
        {
            happiness.Value++;
        }
    }
}