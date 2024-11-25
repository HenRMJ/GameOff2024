using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

partial struct ChangeNameSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences references = SystemAPI.GetSingleton<EntitiesReferences>();

        new ChangeNameJob
        {
            EntitiesReferences = references
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ChangeNameJob : IJobEntity
{
    public EntitiesReferences EntitiesReferences;
    
    public void Execute(ref Name name, Entity entity)
    {
        if (name.InitiatedName) return;

        name.InitiatedName = true;
        
        Random random = new Random((uint)entity.Index);

        random.NextInt();
        
        ref NameBlob Names = ref EntitiesReferences.NamesBlob.Value;
        
        name.FirstName = Names.FirstNames[random.NextInt(0, Names.FirstNames.Length)];
        name.LastName = Names.LastNames[random.NextInt(0, Names.LastNames.Length)];
    }
}
