using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SchoolAuthoring : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float timeToSpawn;
    
    public class Baker : Baker<SchoolAuthoring>
    {
        public override void Bake(SchoolAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new School
            {
                SpawnPosition = authoring.spawnPosition.position,
                TimeToSpawn = authoring.timeToSpawn,
                CultistToSpawn = CultistTypes.Follower,
                StopSpawn = false
            });
        }
    }
}

public struct School : IComponentData
{
    public int Children;
    public bool StopSpawn;
    public float3 SpawnPosition;
    public float ProgressTimer;
    public float TimeToSpawn;
    public CultistTypes CultistToSpawn;
}
