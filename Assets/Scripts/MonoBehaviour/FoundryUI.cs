using TMPro;
using Unity.Entities;
using UnityEngine;

public class FoundryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;

    private EntityManager _entityManager;
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(Foundry));
        Foundry foundry = entityQuery.GetSingleton<Foundry>();

        woodText.text = foundry.Wood.ToString();
        stoneText.text = foundry.Stone.ToString();
    }
}
