using ProjectDawn.Navigation;
using Rukhanka;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using AnimatorParametersAspect = Rukhanka.AnimatorParametersAspect;

partial struct MoverSystem : ISystem
{
    private ComponentLookup<LocalTransform> _localTransformLookup;
    private FastAnimatorParameter _speedParam;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _speedParam = new FastAnimatorParameter("speed");
        _localTransformLookup = state.GetComponentLookup<LocalTransform>(true);
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _localTransformLookup.Update(ref state);
        
        MoverJob moverJob = new MoverJob
        {
            SpeedParam = _speedParam,
            LocalTransformLookup = _localTransformLookup
        };
        moverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct MoverJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformLookup;
    public FastAnimatorParameter SpeedParam;
    
    public void Execute(AnimatorParametersAspect animator, ref SetTarget target, ref AgentBody body, in TargetOverride targetOverride)
    {
        animator.SetParameterValue(SpeedParam, body.Speed);

        if (targetOverride.TargetEntity != Entity.Null)
        {
            target.TargetPosition = LocalTransformLookup[targetOverride.TargetEntity].Position;
        }
        
        if (!math.all(target.TargetPosition == body.Destination))
        {
            body.SetDestination(target.TargetPosition);
        }
        
    }
}