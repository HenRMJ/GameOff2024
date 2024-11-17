using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

partial struct FeedingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Kitchen>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity kitchenEntity = SystemAPI.GetSingletonEntity<Kitchen>();

        NativeList<int> totalMeatConsumed = new NativeList<int>(Allocator.TempJob);
        NativeList<int> totalWaterConsumed = new NativeList<int>(Allocator.TempJob);
        NativeList<int> totalVegetablesConsumed = new NativeList<int>(Allocator.TempJob);

        GatherRequirements gatherRequirements = new GatherRequirements
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            MeatConsumed = totalMeatConsumed.AsParallelWriter(),
            WaterConsumed = totalWaterConsumed.AsParallelWriter(),
            VegetablesConsumed = totalVegetablesConsumed.AsParallelWriter()
        };

        JobHandle gatherRequirementsHandle = gatherRequirements.ScheduleParallel(state.Dependency);
        gatherRequirementsHandle.Complete();

        Kitchen kitchen = SystemAPI.GetComponent<Kitchen>(kitchenEntity);

        int totalMeat = 0;
        int totalWater = 0;
        int totalVegetables = 0;
        
        for (int i = 0; i < totalMeatConsumed.Length; i++)
        {
            totalMeat += totalMeatConsumed[i];
            totalWater += totalWaterConsumed[i];
            totalVegetables += totalVegetablesConsumed[i];
        }

        bool requirementsMet = totalMeat <= kitchen.Meat &&
                               totalWater <= kitchen.Water &&
                               totalVegetables <= kitchen.Vegetables;
        
        if (requirementsMet)
        {
            kitchen.Meat -= totalMeat;
            kitchen.Water -= totalWater;
            kitchen.Vegetables -= totalVegetables;
        }

        new SatiateJob
        {
            RequirementsMet = requirementsMet
        }.ScheduleParallel();
        
        SystemAPI.SetSingleton(kitchen);
        
        totalVegetablesConsumed.Dispose();
        totalMeatConsumed.Dispose();
        totalWaterConsumed.Dispose();
    }
}

[BurstCompile]
public partial struct GatherRequirements : IJobEntity
{
    public float DeltaTime;
    
    public NativeList<int>.ParallelWriter MeatConsumed;
    public NativeList<int>.ParallelWriter WaterConsumed;
    public NativeList<int>.ParallelWriter VegetablesConsumed;
    
    public void Execute(ref NourishmentReceiver nourishmentReceiver)
    {
        if (nourishmentReceiver.Satiated) return;
            
        nourishmentReceiver.timer += DeltaTime;
        if (nourishmentReceiver.timer <= nourishmentReceiver.maxTime) return;
        nourishmentReceiver.timer = 0f;

        nourishmentReceiver.Contributed = true;
        
        MeatConsumed.AddNoResize(nourishmentReceiver.MeatRequirment);
        WaterConsumed.AddNoResize(nourishmentReceiver.WaterRequirement);
        VegetablesConsumed.AddNoResize(nourishmentReceiver.VegetableRequirement);
    }
}

[BurstCompile]
public partial struct SatiateJob : IJobEntity
{
    public bool RequirementsMet;
    
    public void Execute(ref NourishmentReceiver nourishmentReceiver)
    {
        if (!nourishmentReceiver.Contributed) return;

        if (RequirementsMet)
        {
            nourishmentReceiver.Satiated = true;
        }

        nourishmentReceiver.Contributed = false;
    }
}