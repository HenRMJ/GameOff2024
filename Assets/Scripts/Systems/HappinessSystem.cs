using Unity.Burst;
using Unity.Entities;

partial struct HappinessSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new CheckHungerJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
}

public partial struct CheckHungerJob : IJobEntity
{
    public float DeltaTime;
    
    public void Execute(ref Happiness happiness, ref NourishmentReceiver nourishmentReceiver)
    {
        happiness.timer += DeltaTime;
        if (happiness.timer <= happiness.maxTimer) return;
        happiness.timer = 0f;

        happiness.happiness += nourishmentReceiver.Satiated ? 1f : -1f;
            
        nourishmentReceiver.Satiated = false;
    }
}