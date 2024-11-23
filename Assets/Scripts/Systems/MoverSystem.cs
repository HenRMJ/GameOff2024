using Rukhanka;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MoverSystem : ISystem
{
    public const float REACH_TARGET_SQ = 1f;

    private FastAnimatorParameter speedParam;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        speedParam = new FastAnimatorParameter("speed");
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MoverJob moverJob = new MoverJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            SpeedParam = speedParam
        };
        moverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct MoverJob : IJobEntity
{
    public float DeltaTime;
    public FastAnimatorParameter SpeedParam;
    public void Execute(ref LocalTransform localTransform, ref Mover mover, ref PhysicsVelocity velocity, AnimatorParametersAspect animator)
    {
        float3 moveDirection = mover.TargetPosition - localTransform.Position;

        float reachedTargetSq = MoverSystem.REACH_TARGET_SQ;

        if (math.lengthsq(moveDirection) <= reachedTargetSq)
        {
            velocity.Linear = float3.zero;
            velocity.Angular = float3.zero;
            mover.IsMoving = false;
            animator.SetParameterValue(SpeedParam, 0f);
            return;
        }

        mover.IsMoving = true;
        
        moveDirection = math.normalize(moveDirection);

        localTransform.Rotation = math.slerp(localTransform.Rotation, 
            quaternion.LookRotation(moveDirection, math.up()),
            DeltaTime * mover.RotationalSpeed);

        animator.SetParameterValue(SpeedParam, 1f);
        
        velocity.Linear = moveDirection * mover.MoveSpeed;
        velocity.Angular = float3.zero;
        
        
    }
}