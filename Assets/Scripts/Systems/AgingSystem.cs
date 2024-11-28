using Unity.Burst;
using Unity.Entities;

partial struct AgingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new AgingJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct AgingJob : IJobEntity
{
    public float DeltaTime;
    
    public void Execute(ref Age age)
    {
        age.Timer += DeltaTime;
        if (age.Timer < age.TimerToAge) return;
        age.Timer = 0f;

        age.Value++;
    }
}