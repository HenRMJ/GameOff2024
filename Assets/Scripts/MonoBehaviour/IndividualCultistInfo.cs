using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class IndividualCultistInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text cultistName;
    [SerializeField] private TMP_Text age;
    [SerializeField] private Image healthBar;

    [Header("Update Values")] 
    [SerializeField] private float updateInterval;

    private EntityManager _entityManager;
    private Entity _entity;
    private float _timer;
    private CultResources _resourceToSearchFor;

    public void Initalize(Entity entity, string name, string age)
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _entity = entity;
        this.cultistName.text = name;
        this.age.text = age;
        _resourceToSearchFor = CultResources.None;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;
        _timer = 0f;

        if (!_entityManager.Exists(_entity))
        {
            Destroy(gameObject);
            return;
        }
        
        Health health = _entityManager.GetComponentData<Health>(_entity);
        healthBar.fillAmount = (float)health.Value / health.MaxHealth;
        
        FindClosestTarget();
    }

    public void SetSearchResource(CultResources resourceToSearchFor)
    {
        _resourceToSearchFor = resourceToSearchFor;
    }
    
    private void FindClosestTarget()
    {
        RefRW<CultResource> resource = _entityManager.GetComponentDataRW<CultResource>(_entity);
        
        if (resource.ValueRO.Resource == CultResources.None ||
            resource.ValueRO.Resource == CultResources.Meat)
        {
            RefRW<SetTarget> target = _entityManager.GetComponentDataRW<SetTarget>(_entity);
            LocalTransform localTransform = _entityManager.GetComponentData<LocalTransform>(_entity);

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<CultResource, ResourceContainer, LocalTransform>().Build(_entityManager);
            NativeArray<CultResource> resources = entityQuery.ToComponentDataArray<CultResource>(Allocator.Temp);
            NativeArray<LocalTransform> resourceTransforms =
                entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

            float3 closestPosition = localTransform.Position;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].Resource != _resourceToSearchFor) continue;

                float3 entityPosition = resourceTransforms[i].Position;
                float distanceSq = math.distancesq(localTransform.Position, entityPosition);

                if (distanceSq < closestDistance)
                {
                    closestDistance = distanceSq;
                    closestPosition = entityPosition;
                }
            }

            resources.Dispose();
            resourceTransforms.Dispose();

            target.ValueRW.TargetPosition = closestPosition;
        }
    }
}
