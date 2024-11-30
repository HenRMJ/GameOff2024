using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class KitchenUI : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private TMP_Text vegetableRequirementText;
    [SerializeField] private TMP_Text meatRequirementText;
    [SerializeField] private TMP_Text waterRequirementText;
    
    [Header("Current")]
    [SerializeField] private TMP_Text vegetableCurrentText;
    [SerializeField] private TMP_Text meatCurrentText;
    [SerializeField] private TMP_Text waterCurrentText;
    
    private EntityManager _entityManager;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(NourishmentReceiver));

        int vegRequirements = 0;
        int meatRequirements = 0;
        int waterRequirements = 0;
        
        Entity[] hungerEntities = entityQuery.ToEntityArray(Allocator.Temp).ToArray();
        for (int i = 0; i < hungerEntities.Length; i++)
        {
            NourishmentReceiver nourishmentReceiver = _entityManager.GetComponentData<NourishmentReceiver>(hungerEntities[i]);

            vegRequirements += nourishmentReceiver.VegetableRequirement;
            meatRequirements += nourishmentReceiver.MeatRequirment;
            waterRequirements += nourishmentReceiver.WaterRequirement;
        }

        vegetableRequirementText.text = vegRequirements.ToString();
        meatRequirementText.text = meatRequirements.ToString();
        waterRequirementText.text = waterRequirements.ToString();

        entityQuery = _entityManager.CreateEntityQuery(typeof(Kitchen));
        Kitchen kitchen = entityQuery.GetSingleton<Kitchen>();

        meatCurrentText.text = kitchen.Meat.ToString();
        waterCurrentText.text = kitchen.Water.ToString();
        vegetableCurrentText.text = kitchen.Vegetables.ToString();
    }
}
