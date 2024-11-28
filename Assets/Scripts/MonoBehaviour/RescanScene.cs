using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;
using SphereCollider = UnityEngine.SphereCollider;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public class RescanScene : MonoBehaviour
{
    public static void Rescan()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(LocalTransform), typeof(PhysicsCollider), typeof(BuildingType));

        int ColliderIndex = 0;
        
        NativeArray<PhysicsCollider> colliders = entityQuery.ToComponentDataArray<PhysicsCollider>(Allocator.Temp);
        NativeArray<LocalTransform> transforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (PhysicsCollider collider in colliders)
        {
            GameObject tempCollider = new GameObject("tempCollider");
            
            if (collider.Value.Value.Type == ColliderType.Box) {
                unsafe
                {
                    BoxCollider boxCollider = tempCollider.AddComponent<BoxCollider>();
                
                    BoxGeometry boxGeometry = ((Unity.Physics.BoxCollider*)collider.ColliderPtr)->Geometry;
                    // Set the bounds based on the collider's dimensions
                    boxCollider.size = boxGeometry.Size;
                    boxCollider.center = boxGeometry.Center;
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
            gameObjects.Add(tempCollider);
        }

        AstarPath.active.Scan();

        foreach (GameObject temporarycollider in gameObjects)
        {
            Destroy(temporarycollider);
        }

        colliders.Dispose();
    }
}
