using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject brothelGhost;

    private bool _readyPlacement;

    private VisualPlacementIndicator _indicator;
    private EntityManager _entityManager;

    private int _amountOfWoodToDeduct;
    private int _amountOfStoneToDeduct;
    
    private void Start()
    {
        _indicator = brothelGhost.GetComponent<VisualPlacementIndicator>();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    
    private void Update()
    {
        if (!_readyPlacement)
        {
            if (brothelGhost.activeInHierarchy)
            {
                brothelGhost.SetActive(false);
            }
            return;
        }

        if (!brothelGhost.activeInHierarchy)
        {
            brothelGhost.SetActive(true);
        }

        Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
        
        brothelGhost.transform.position = mouseWorldPosition;

        if (Input.GetMouseButtonDown(1))
        {
            ResetPlacement();
        }
        
        if (!_indicator.CanPlace()) return;

        if (Input.GetMouseButtonDown(0))
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(EntitiesReferences));
            EntitiesReferences entitiesReferences = entityQuery.GetSingleton<EntitiesReferences>();
            Entity spawnedEntity = _entityManager.Instantiate(entitiesReferences.BrothelEntity);
            
            Debug.Log(mouseWorldPosition);
            
            _entityManager.SetComponentData(spawnedEntity, new LocalTransform
            {
                Position = mouseWorldPosition,
                Rotation =  quaternion.identity,
                Scale = 1f
            });

            entityQuery = _entityManager.CreateEntityQuery(typeof(Foundry));
            Foundry foundry = entityQuery.GetSingleton<Foundry>();
            foundry.Wood -= _amountOfWoodToDeduct;
            foundry.Stone -= _amountOfStoneToDeduct;
            entityQuery.SetSingleton(foundry);
            
            Debug.Log(_entityManager.GetComponentData<LocalTransform>(spawnedEntity).Position);
            StartCoroutine(CheckEntityLocation(spawnedEntity));
            ResetPlacement();
        }
    }

    private IEnumerator CheckEntityLocation(Entity entityToCheck)
    {
        yield return new WaitForSeconds(1f);

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        Debug.Log(_entityManager.GetComponentData<LocalTransform>(entityToCheck).Position);
        Debug.Log(entityToCheck);
    }
    
    public void ReadyPlacement(int woodCost, int stoneCost)
    {
        _readyPlacement = true;
        _amountOfStoneToDeduct = stoneCost;
        _amountOfWoodToDeduct = woodCost;
    }

    private void ResetPlacement()
    {
        _readyPlacement = false;
            
        _amountOfStoneToDeduct = 0;
        _amountOfWoodToDeduct = 0;
    }
}