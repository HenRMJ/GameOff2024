using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class AvatarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text devotionText;
    
    [Header("Nourishment Values")]
    [SerializeField] private Button nourishmentButton;
    [SerializeField] private int nourishmentCost;
    [SerializeField] private float nourishmentTime;
    
    private EntityManager _entityManager;
    private bool _godlyNourishment;
    private bool _initialized;
    private float _timer;
    
    private void OnEnable()
    {
        if (!_initialized)
        {
            _initialized = true;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        CheckGodlyNourishment();
    }
    
    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        nourishmentButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{nourishmentCost.ToString()} XP";
    }

    private void Update()
    {
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(Avatar));
        Avatar avatar = entityQuery.GetSingleton<Avatar>();

        devotionText.text = avatar.Devotion.ToString();

        if (_godlyNourishment)
        {
            _timer += Time.deltaTime;
            if (_timer > 1f)
            {
                _timer = 0f;
                CheckGodlyNourishment();
            }
        }
        
        bool canInteractWith = avatar.Devotion >= nourishmentCost && !_godlyNourishment;

        if (canInteractWith != nourishmentButton.IsInteractable())
        {
            nourishmentButton.interactable = canInteractWith;
        }
    }
    
    public void EnableNourishment()
    {
        _godlyNourishment = true;
        RefRW<Avatar> avatar = _entityManager.CreateEntityQuery(typeof(Avatar)).GetSingletonRW<Avatar>();
        RefRW<Kitchen> kitchen = _entityManager.CreateEntityQuery(typeof(Kitchen)).GetSingletonRW<Kitchen>();

        kitchen.ValueRW.GodlyNourishment = true;
        avatar.ValueRW.Devotion -= nourishmentCost;
    }

    private void CheckGodlyNourishment()
    {
        EntityQuery kitchenQuery = _entityManager.CreateEntityQuery(typeof(Kitchen));

        if (kitchenQuery.TryGetSingleton(out Kitchen kitchen))
        {
            _godlyNourishment = kitchen.GodlyNourishment;
        }
    }
}
