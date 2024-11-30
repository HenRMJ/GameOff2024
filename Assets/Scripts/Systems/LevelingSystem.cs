using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

partial struct LevelingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Avatar>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Avatar avatar = SystemAPI.GetSingleton<Avatar>();

        new LevelingJob
        {
            BaseLevel = avatar.BaseContributionCurve

        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct LevelingJob : IJobEntity
{
    public int BaseLevel;
    public void Execute(ref Cultist cultist)
    {
        if (!Mathf.Approximately(cultist.Devotion, cultist.PreviousDevotion))
        {
            cultist.PreviousDevotion = cultist.Devotion;
            cultist.Level = (int)(BaseLevel * math.log10(cultist.Devotion + 1));
        }
    }
}