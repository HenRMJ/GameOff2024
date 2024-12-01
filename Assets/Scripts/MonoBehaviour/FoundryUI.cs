using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class FoundryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;

    [Header("Fertility Cost")] 
    [SerializeField] private int woodCost;
    [SerializeField] private int stoneCost;
    [SerializeField] private TMP_Text woodCostText;
    [SerializeField] private TMP_Text stoneCostText;
    [SerializeField] private Button purchaseButton;

    [Header("External References")] 
    [SerializeField] private BuildingPlacementManager _buildingManager;
    
    private EntityManager _entityManager;
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        woodCostText.text = woodCost.ToString();
        stoneCostText.text = stoneCost.ToString();
    }

    private void Update()
    {
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(Foundry));
        if (entityQuery.TryGetSingleton(out Foundry foundry))
        {
            woodText.text = foundry.Wood.ToString();
            stoneText.text = foundry.Stone.ToString();

            purchaseButton.interactable = foundry.Stone >= stoneCost && foundry.Wood >= woodCost;
        }
    }

    public void PurchaseBuilding()
    {
        _buildingManager.ReadyPlacement(woodCost, stoneCost);
    }
}
