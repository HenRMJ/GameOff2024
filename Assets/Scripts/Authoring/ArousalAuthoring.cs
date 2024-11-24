using Unity.Entities;
using UnityEngine;

public class ArousalAuthoring : MonoBehaviour
{
    [SerializeField] private int SeekValue;
    [SerializeField] private int HappinessThresshold;
    [SerializeField] private float ReproductionRangeSq;
    [SerializeField] private float CheckTimer;
    [SerializeField] private float SeekDistance;
    
    public class Baker : Baker<ArousalAuthoring>
    {
        public override void Bake(ArousalAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Arousal
            {
                SeekValue = authoring.SeekValue,
                HappinessThreshold = authoring.HappinessThresshold,
                CheckTimer = authoring.CheckTimer,
                ReproductionRangeSq = authoring.ReproductionRangeSq,
                SeekDistance = authoring.SeekDistance
            });
        }
    }
}

public struct Arousal : IComponentData
{
    public Entity Partner;
    public int ArousalValue;
    public int SeekValue;
    public int HappinessThreshold;
    public float ReproductionRangeSq;
    public float SeekDistance;
    public float CheckTimer;
    public float Timer;
}
