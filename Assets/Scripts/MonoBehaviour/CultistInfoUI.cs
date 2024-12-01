using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CultistInfoUI : MonoBehaviour
{
    private void Start()
    {
        SelectionManager.Instance.OnSelectedEntitiesChanged += SelectionManager_OnSelectedEntitiesChanged;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SelectionManager.Instance.OnSelectedEntitiesChanged -= SelectionManager_OnSelectedEntitiesChanged;
    }

    private void SelectionManager_OnSelectedEntitiesChanged(object sender, EventArgs e)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, Name, Age, Health>().Build(entityManager);

        NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<Name> nameArray = entityQuery.ToComponentDataArray<Name>(Allocator.Temp);
        NativeArray<Age> ageArray = entityQuery.ToComponentDataArray<Age>(Allocator.Temp);
        NativeArray<Health> healthArray = entityQuery.ToComponentDataArray<Health>(Allocator.Temp);

        if (transform.childCount > 1)
        {
            foreach (Transform child in transform)
            {
                if (transform.GetChild(0) == child) continue;

                Destroy(child.gameObject);
            }
        }

        gameObject.SetActive(false);
        if (nameArray.Length <= 0) return;
        gameObject.SetActive(true);

        for (int i = 0; i < nameArray.Length; i++)
        {
            Transform spawnedCultistPanel = Instantiate(transform.GetChild(0), transform);
            IndividualCultistInfo info = spawnedCultistPanel.GetComponent<IndividualCultistInfo>();
            info.Initalize(entities[i], 
                $"{nameArray[i].FirstName} {nameArray[i].LastName}", 
                ageArray[i].Value.ToString(),
                (float)healthArray[i].Value / healthArray[i].MaxHealth);

            spawnedCultistPanel.gameObject.SetActive(true);
        }
    }
}
