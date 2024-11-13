using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct TestJob : IJob
{
    public float3 position;
    public float speed;

    public void Execute()
    {
        position += new float3(0, 0, speed);
    }
}
