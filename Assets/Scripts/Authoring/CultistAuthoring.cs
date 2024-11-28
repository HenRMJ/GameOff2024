using Unity.Entities;
using UnityEngine;

public class CultistAuthoring : MonoBehaviour
{
    [SerializeField] private float _devotion;
    
    public class Baker : Baker<CultistAuthoring>
    {
        public override void Bake(CultistAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Cultist
            {
                Devotion = authoring._devotion,
            });
        }
    }
}

public struct Cultist : IComponentData
{
    public int Level;
    public float Devotion;
    public float PreviousDevotion;
}