using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Collections;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;
using SphereCollider = UnityEngine.SphereCollider;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public class RescanScene : MonoBehaviour
{
    private static Dictionary<GameObject, bool> _existingGameObjects = new();

    private void Start()
    {
        AstarPath.active.Scan();
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

            if (collider.Value.Value.Type == ColliderType.Box) {
                unsafe
                {
                    BoxCollider boxCollider = tempCollider.AddComponent<BoxCollider>();
                    BoxGeometry boxGeometry = ((Unity.Physics.BoxCollider*)collider.ColliderPtr)->Geometry;
                    // Set the bounds based on the collider's dimensions
                    boxCollider.size = boxGeometry.Size;
                    boxCollider.center = boxGeometry.Center;
                    NavmeshCut cut = tempCollider.AddComponent<NavmeshCut>();
                    cut.center = boxCollider.center;
                    cut.rectangleSize = new Vector2(boxGeometry.Size.x, boxGeometry.Size.z);
                    cut.height = 10f;
                }
            }
            else if (collider.Value.Value.Type == ColliderType.Sphere) {
                unsafe
                {
                    SphereCollider sphereCollider = tempCollider.AddComponent<SphereCollider>();
                    
                    // Set the radius and position based on the collider's dimensions
                    SphereGeometry sphereGeometry = ((Unity.Physics.SphereCollider*)collider.ColliderPtr)->Geometry;
                    sphereCollider.radius = sphereGeometry.Radius;
                    sphereCollider.center = sphereGeometry.Center;
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