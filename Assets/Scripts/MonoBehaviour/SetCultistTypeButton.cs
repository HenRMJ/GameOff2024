using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SetCultistTypeButton : MonoBehaviour
{
    private static EventHandler _updatedType;
    
    [SerializeField] private CultistTypes typeToSpawn;

    private EntityManager _entityManager;
    private Button _button;
    private Sprite _defaultSprite;

    private void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _button = GetComponent<Button>();
        _defaultSprite = (_button.targetGraphic as Image)?.sprite;
    }

    private void OnEnable()
    {
        UpdateSpriteState(this, EventArgs.Empty);
    }

    private void UpdateSpriteState(object sender, EventArgs e)
    {
        EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(School));
        if (!entityQuery.TryGetSingleton<School>(out School school)) return;

        _button.image.sprite = _defaultSprite;
        
        if (school.CultistToSpawn == typeToSpawn)
        {
            _button.image.sprite = _button.spriteState.selectedSprite;
        }
    }

    private void Start()
    {
        _updatedType += UpdateSpriteState;
        
        _button.onClick.AddListener(() =>
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = _entityManager.CreateEntityQuery(typeof(School));
            if (!entityQuery.TryGetSingleton<School>(out School school)) return;

            EventSystem.current.SetSelectedGameObject(_button.gameObject);

            school.CultistToSpawn = typeToSpawn;

            _entityManager.SetComponentData(entityQuery.GetSingletonEntity(), school);
            _updatedType?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnDestroy()
    {
        _updatedType -= UpdateSpriteState;
    }
}
