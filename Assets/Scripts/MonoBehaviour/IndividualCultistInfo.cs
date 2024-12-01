using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IndividualCultistInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text cultistName;
    [FormerlySerializedAs("age")] [SerializeField] private TMP_Text cultistAge;
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Dropdown dropdown;
    
    [Header("Update Values")] 
    [SerializeField] private float updateInterval;

    private EntityManager _entityManager;
    private Entity _entity;
    private float _timer;
    private CultResources _resourceToSearchFor;

    public void Initalize(Entity entity, string nameString, string age, float normalizedHealth)
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _entity = entity;
        cultistName.text = nameString;
        cultistAge.text = age;
        _resourceToSearchFor = _entityManager.GetComponentData<CultResource>(entity).Resource;
        dropdown.value = (int)_resourceToSearchFor;
        healthBar.fillAmount = normalizedHealth;
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

    public void SetSearchResource(int resourceToSearchFor)
    {
        _resourceToSearchFor = (CultResources)resourceToSearchFor;
        FindClosestTarget();
    }
    
    private void FindClosestTarget()
    {
        RefRW<CultResource> resource = _entityManager.GetComponentDataRW<CultResource>(_entity);
        RefRW<SetTarget> target = _entityManager.GetComponentDataRW<SetTarget>(_entity);
        RefRW<Productivity> productivity = _entityManager.GetComponentDataRW<Productivity>(_entity);
        LocalTransform localTransform = _entityManager.GetComponentData<LocalTransform>(_entity);
        
        if (_resourceToSearchFor != CultResources.None)
        {
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<CultResource, ResourceContainer, LocalTransform>().Build(_entityManager);
            NativeArray<CultResource> resources = entityQuery.ToComponentDataArray<CultResource>(Allocator.Temp);
            NativeArray<LocalTransform> resourceTransforms =
                entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            NativeArray<Entity> resourceEntities = entityQuery.ToEntityArray(Allocator.Temp);

            float3 closestPosition = localTransform.Position;
            Entity closestEntity = Entity.Null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].Resource != _resourceToSearchFor) continue;

                float3 entityPosition = resourceTransforms[i].Position;
                float distanceSq = math.distance(localTransform.Position, entityPosition);

                if (distanceSq < closestDistance)
                {
                    closestDistance = distanceSq;
                    closestPosition = entityPosition;
                    closestEntity = resourceEntities[i];
                }
            }

            resources.Dispose();
            resourceTransforms.Dispose();

            if (closestEntity == Entity.Null) return;
            
            target.ValueRW.TargetPosition = closestPosition;
            productivity.ValueRW.ContributationEntity = closestEntity;
            resource.ValueRW.Resource = _resourceToSearchFor;
        }
        else
        {
            resource.ValueRW.Resource = CultResources.None;
            productivity.ValueRW.ContributationEntity = Entity.Null;
        }
    }
}
