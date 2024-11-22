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
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, Name>().Build(entityManager);

        NativeArray<Name> nameArray = entityQuery.ToComponentDataArray<Name>(Allocator.Temp);

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
            spawnedCultistPanel.GetChild(0).GetComponent<TMP_Text>().text =
                $"{nameArray[i].FirstName} {nameArray[i].LastName}";

            spawnedCultistPanel.gameObject.SetActive(true);
        }
    }
}
