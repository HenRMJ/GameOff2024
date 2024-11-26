using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;

partial struct SchoolSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<School>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        School school = SystemAPI.GetSingleton<School>();
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        if (school.StopSpawn) return;
        if (school.Children <= 0) return;
        
        school.ProgressTimer += SystemAPI.Time.DeltaTime;
        
        if (school.ProgressTimer >= school.TimeToSpawn)
        {
            school.ProgressTimer = 0f;

            Entity entityToSpawn = Entity.Null;
            
            switch (school.CultistToSpawn)
            {
                case CultistTypes.Follower:
                    entityToSpawn = entitiesReferences.FollowerCultistEntity;
                    break;
                case CultistTypes.Builder:
                    entityToSpawn = entitiesReferences.BuilderCultistEntity;
                    break;
                case CultistTypes.Farmer:
                    entityToSpawn = entitiesReferences.FarmerCultistEntity;
                    break;
            }

            Entity spawnedEntity = state.EntityManager.Instantiate(entityToSpawn);

            LocalTransform spawnedTransform = SystemAPI.GetComponent<LocalTransform>(spawnedEntity);
            spawnedTransform.Position = school.SpawnPosition;
            
            SystemAPI.SetComponent(spawnedEntity, spawnedTransform);
            
            school.Children--;
        }
        
        SystemAPI.SetSingleton(school);
    }
}
