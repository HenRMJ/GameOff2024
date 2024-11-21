using Unity.Entities;
using UnityEngine;

public class NourishmentReceiverAuthoring : MonoBehaviour
{
    [SerializeField] private int _meatRequirment;
    [SerializeField] private int _waterRequirement;
    [SerializeField] private int _vegetableRequirement;
    [SerializeField] private float _maxTime;
    
    public class Baker : Baker<NourishmentReceiverAuthoring>
    {
        public override void Bake(NourishmentReceiverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NourishmentReceiver
            {
                MeatRequirment = authoring._meatRequirment,
                WaterRequirement = authoring._waterRequirement,
                VegetableRequirement = authoring._vegetableRequirement,
                maxTime = authoring._maxTime
            });
        }
    }
}

public struct NourishmentReceiver : IComponentData
{
    public int MeatRequirment;
    public int WaterRequirement;
    public int VegetableRequirement;
    public float timer;
    public float maxTime;
    public bool Satiated;
    public bool Contributed;
}
