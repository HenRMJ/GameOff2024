using System;
using System.Xml;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;
    public event EventHandler OnSelectedEntitiesChanged;

    private Vector2 selectionStartMousePosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is already an instance of the Selection Manager in the scene");
            Destroy(this);
            return;
        }
        
        Instance = this;
    }

    private void Update()
    {
        // if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                for (int i = 0; i < entityArray.Length; i++)
                {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                    Selected selected = selectedArray[i];
                    selected.onDeselected = true;
                    entityManager.SetComponentData(entityArray[i], selected);
                }
            }

            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width * selectionAreaRect.height;
            float multipleSelectionSizeMin = 40f;
            bool multipleSelections = selectionAreaSize > multipleSelectionSizeMin;

            if (multipleSelections)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Mover>()
                    .WithPresent<Selected>().Build(entityManager);

                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransformArray =
                    entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                for (int i = 0; i < localTransformArray.Length; i++)
                {
                    LocalTransform unitLocalTransform = localTransformArray[i];

                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);

                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                        Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                        selected.onSelected = true;
                        entityManager.SetComponentData(entityArray[i], selected);
                    }
                }
            }
            else
            {
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(999f),
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << 6,
                        GroupIndex = 0
                    }
                };

                if (collisionWorld.CastRay(raycastInput, out RaycastHit raycastHit))
                {
                    if (entityManager.HasComponent<Selected>(raycastHit.Entity))
                    {
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                        Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                        selected.onSelected = true;
                        entityManager.SetComponentData(raycastHit.Entity, selected);
                    }
                }
            }
            
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            OnSelectedEntitiesChanged?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0f),
                End = cameraRay.GetPoint(999f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << 7,
                    GroupIndex = 0
                }
            };

            bool selectedBuilding = false;

            if (collisionWorld.CastRay(raycastInput, out RaycastHit raycastHit))
            {
                
            }
        }
    }

    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new(Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));

        Vector2 upperRightCorner = new(Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));

        return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y);
    }
}
