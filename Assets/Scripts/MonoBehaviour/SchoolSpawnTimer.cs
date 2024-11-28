using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class SchoolSpawnTimer : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private Toggle spawnToggle;
    [SerializeField] private TMP_Text numberOfChildren;
    
    private EntityManager _entityManager;
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(School));
        School school = entityQuery.GetSingleton<School>();

        spawnToggle.isOn = !school.StopSpawn;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(School));
        School school = entityQuery.GetSingleton<School>();

        numberOfChildren.text = school.Children.ToString();
        
        if (school.StopSpawn) return;

        progressBar.fillAmount = school.ProgressTimer / school.TimeToSpawn;
    }

    public void ToggleSpawn()
    {
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(School));
        School school = entityQuery.GetSingleton<School>();

        school.StopSpawn = !spawnToggle.isOn;
        
        _entityManager.SetComponentData(entityQuery.GetSingletonEntity(), school);
    }
}
