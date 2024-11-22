using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MoverSystem : ISystem
{
    public const float REACH_TARGET_SQ = 1f;
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MoverJob moverJob = new MoverJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        moverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct MoverJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(ref LocalTransform localTransform, ref Mover mover, ref PhysicsVelocity velocity)
    {
        float3 moveDirection = mover.TargetPosition - localTransform.Position;

        float reachedTargetSq = MoverSystem.REACH_TARGET_SQ;

        if (math.lengthsq(moveDirection) <= reachedTargetSq)
        {
            velocity.Linear = float3.zero;
            velocity.Angular = float3.zero;
            mover.IsMoving = false;
            return;
        }

        mover.IsMoving = true;
        
        moveDirection = math.normalize(moveDirection);

        localTransform.Rotation = math.slerp(localTransform.Rotation, 
            quaternion.LookRotation(moveDirection, math.up()),
            DeltaTime * mover.RotationalSpeed);

        velocity.Linear = moveDirection * mover.MoveSpeed;
        velocity.Angular = float3.zero;
    }
}