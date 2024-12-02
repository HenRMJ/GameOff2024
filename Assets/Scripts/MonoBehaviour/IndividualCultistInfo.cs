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

        CultResource resource = _entityManager.GetComponentData<CultResource>(_entity);
        dropdown.value = (int)resource.Resource;
    }

    public void SetSearchResource(int resourceToSearchFor)
    {
        _resourceToSearchFor = (CultResources)resourceToSearchFor;
        RefRW<ForcedWork> forced = _entityManager.GetComponentDataRW<ForcedWork>(_entity);

        if (_resourceToSearchFor != CultResources.None)
        {
            forced.ValueRW.Forced = true;
            forced.ValueRW.ResourceToSearchFor = _resourceToSearchFor;
        }
        else
        {
            forced.ValueRW.Forced = false;
            forced.ValueRW.ResourceToSearchFor = _resourceToSearchFor;
        }
    }
}
