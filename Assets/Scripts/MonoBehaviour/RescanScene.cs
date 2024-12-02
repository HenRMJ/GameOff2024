using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Collections;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.AI;

public class RescanScene : MonoBehaviour
{
    private static Dictionary<GameObject, bool> _existingGameObjects = new();
    private static GameObject parentObject;

    private void Awake()
    {
        parentObject = gameObject;
    }

    public static void Rescan()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(LocalTransform), typeof(PhysicsCollider));

        int ColliderIndex = 0;
        
        NativeArray<PhysicsCollider> colliders = entityQuery.ToComponentDataArray<PhysicsCollider>(Allocator.Temp);
        NativeArray<LocalTransform> transforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        
        MarkAllObjectsForDestruction();
        
        foreach (PhysicsCollider collider in colliders)
        {
            if (GameObjectAlreadyExists(transforms[ColliderIndex].Position))
            {
                ColliderIndex++;
                continue;
            }
            
            GameObject tempCollider = new GameObject("tempCollider");
            tempCollider.layer = LayerMask.NameToLayer("Building");
            tempCollider.transform.parent = parentObject.transform;

            if (collider.Value.Value.Type == ColliderType.Box) {
                unsafe
                {
                    BoxCollider boxCollider = tempCollider.AddComponent<BoxCollider>();
                    BoxGeometry boxGeometry = ((Unity.Physics.BoxCollider*)collider.ColliderPtr)->Geometry;
                    
                    boxCollider.size = boxGeometry.Size;
                    boxCollider.center = boxGeometry.Center;
                    NavMeshObstacle cut = tempCollider.AddComponent<NavMeshObstacle>();
                    cut.center = boxCollider.center;
                    cut.size = boxCollider.size;
                    cut.height = 10f;
                    cut.carving = true;
                }
            }
            
            tempCollider.transform.position = transforms[ColliderIndex].Position;
            ColliderIndex++;
            _existingGameObjects.Add(tempCollider, true);
        }

        List<GameObject> gameObjectsToDestory =
            _existingGameObjects.Where(kv => kv.Value == false).Select(kv => kv.Key).ToList();
        
        foreach (GameObject objectToDestroy in gameObjectsToDestory)
        {
            _existingGameObjects.Remove(objectToDestroy);
            Destroy(objectToDestroy);
        }

        colliders.Dispose();
        transforms.Dispose();
    }

    private static bool GameObjectAlreadyExists(Vector3 position)
    {
        foreach (GameObject collider in _existingGameObjects.Keys.ToList())
        {
            if (collider == null)
            {
                _existingGameObjects.Remove(collider);
                continue;
            }
            
            if (collider.transform.position == position)
            {
                _existingGameObjects[collider] = true;
                return true;
            }
        }

        return false;
    }

    private static void MarkAllObjectsForDestruction()
    {
        foreach (GameObject key in _existingGameObjects.Keys.ToList())
        {
            _existingGameObjects[key] = false;
        }
    }
}