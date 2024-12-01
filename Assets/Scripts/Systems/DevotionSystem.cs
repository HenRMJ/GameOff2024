using Rukhanka;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct DevotionSystem : ISystem
{
    private CollisionFilter _collisionFilter;
    private FastAnimatorParameter devoteTrigger;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Avatar>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        _collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << 6,
            GroupIndex = 0
        };

        devoteTrigger = new FastAnimatorParameter("devote");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState sate)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        Avatar avatar = SystemAPI.GetSingleton<Avatar>();
        
        avatar.Timer += SystemAPI.Time.DeltaTime;
        if (avatar.Timer < avatar.DevotionTimer)
        {
            SystemAPI.SetSingleton(avatar);
            return;
        }
        avatar.Timer = 0f;
        
        Entity avatarEntity = SystemAPI.GetSingletonEntity<Avatar>();
        LocalTransform localTransform = SystemAPI.GetComponent<LocalTransform>(avatarEntity);

        if (collisionWorld.OverlapSphere(localTransform.Position, avatar.Range, ref distanceHits,
                _collisionFilter))
        {
            foreach (DistanceHit hit in distanceHits)
            {
                if (SystemAPI.Exists(hit.Entity) &&
                    SystemAPI.HasComponent<Cultist>(hit.Entity))
                {
                    Cultist cultist = SystemAPI.GetComponent<Cultist>(hit.Entity);
                    CultistType type = SystemAPI.GetComponent<CultistType>(hit.Entity);
                    
                    cultist.Devotion++;
                    AnimatorParametersAspect animator = SystemAPI.GetAspect<AnimatorParametersAspect>(hit.Entity);
                    animator.SetTrigger(devoteTrigger);
                    
                    avatar.Devotion += type.Type == CultistTypes.Follower ? cultist.Level * 2 : cultist.Level;
                    SystemAPI.SetComponent(hit.Entity, cultist);
                }
            }
        }
        
        SystemAPI.SetSingleton(avatar);
    }
}