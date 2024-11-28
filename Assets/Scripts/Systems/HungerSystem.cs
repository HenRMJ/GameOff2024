using Unity.Burst;
using Unity.Entities;

[UpdateAfter(typeof(FeedingSystem))]
partial struct HungerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new HungerJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct HungerJob : IJobEntity
{
    public float DeltaTime;
    
    public void Execute(ref Hunger hunger, ref NourishmentReceiver receiver)
    {
        hunger.Timer += DeltaTime;
        if (hunger.Timer <= hunger.TimerMax) return;
        hunger.Timer = 0f;

        hunger.Value--;
        
        if (receiver.Satiated)
        {
            hunger.Value += 2;
        }

        if (hunger.Value > hunger.MaxHunger)
        {
            hunger.Value = hunger.MaxHunger;
        }

        if (hunger.Value < 0)
        {
            hunger.Value = 0;
        }

        receiver.Satiated = false;
    }
}