using ProjectDawn.Navigation;
using Rukhanka;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

partial struct RandomGoatWalkingSystem : ISystem
{
    private FastAnimatorParameter _moving;
    private FastAnimatorParameter _graze;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _moving = new FastAnimatorParameter("moving");
        _graze = new FastAnimatorParameter("graze");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (randomWalking, target, body, entity) in SystemAPI
                     .Query<RefRW<RandomGoatWalking>, RefRW<SetTarget>, RefRW<AgentBody>>().WithEntityAccess())
        {
            if (!randomWalking.ValueRO.Initialized)
            {
                randomWalking.ValueRW.Random = new Random((uint)entity.Index);
                randomWalking.ValueRW.Random.NextBool();
                randomWalking.ValueRW.Initialized = true;
            }

            float range = randomWalking.ValueRO.RangeToMove;

            AnimatorParametersAspect animator = SystemAPI.GetAspect<AnimatorParametersAspect>(entity);
            
            animator.SetParameterValue(_moving, !body.ValueRO.IsStopped);
            
            randomWalking.ValueRW.WaitTimer += SystemAPI.Time.DeltaTime;
            if (randomWalking.ValueRO.WaitTimer < randomWalking.ValueRO.WaitInterval) continue;
            randomWalking.ValueRW.WaitTimer = 0f;

            if (randomWalking.ValueRW.Random.NextBool())
            {
                animator.SetTrigger(_graze);
                continue;
            }

            target.ValueRW.TargetPosition = new float3(
                randomWalking.ValueRW.Random.NextFloat(-range, range),
                0f,
                randomWalking.ValueRW.Random.NextFloat(-range, range));

            if (!math.all(target.ValueRO.TargetPosition == body.ValueRO.Destination))
            {
                body.ValueRW.SetDestination(target.ValueRO.TargetPosition);
            }

        }
    }
}
