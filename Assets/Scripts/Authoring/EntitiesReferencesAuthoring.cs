using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject farmerCultistPrefab;
    [SerializeField] private GameObject builderCulstitPrefab;
    [SerializeField] private GameObject followerCultistPrefab;
    [SerializeField] private GameObject brothelPrefab;

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            string[] fNames = NameDatabase.GetFirstNames();
            string[] lNames = NameDatabase.GetLastNames();

            using (var builder = new BlobBuilder(Allocator.Temp))
            {
                ref NameBlob nameBlob = ref builder.ConstructRoot<NameBlob>();

                BlobBuilderArray<FixedString32Bytes> firstNamesArray =
                    builder.Allocate(ref nameBlob.FirstNames, fNames.Length);
                BlobBuilderArray<FixedString32Bytes> lastNamesArray =
                    builder.Allocate(ref nameBlob.LastNames, lNames.Length);

                for (int i = 0; i < fNames.Length; i++)
                {
                    firstNamesArray[i] = new FixedString32Bytes(fNames[i]);
                }

                for (int i = 0; i < lNames.Length; i++)
                {
                    lastNamesArray[i] = new FixedString32Bytes(lNames[i]);
                }

                BlobAssetReference<NameBlob> namesBlob = builder.CreateBlobAssetReference<NameBlob>(Allocator.Persistent);
                
                AddComponent(entity, new EntitiesReferences
                {
                    BuilderCultistEntity = GetEntity(authoring.builderCulstitPrefab, TransformUsageFlags.Dynamic),
                    FarmerCultistEntity = GetEntity(authoring.farmerCultistPrefab, TransformUsageFlags.Dynamic),
                    FollowerCultistEntity = GetEntity(authoring.followerCultistPrefab, TransformUsageFlags.Dynamic),
                    BrothelEntity = GetEntity(authoring.brothelPrefab, TransformUsageFlags.Dynamic),
                    NamesBlob = namesBlob,
                    Random = new Random((uint)System.Environment.TickCount)
                });
                
                AddComponent(entity, new CleanupNameBlob
                {
                    NamesBlob = namesBlob
                });
            }
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity BuilderCultistEntity;
    public Entity FarmerCultistEntity;
    public Entity FollowerCultistEntity;
    public Entity BrothelEntity;
    public BlobAssetReference<NameBlob> NamesBlob;
    public Random Random;
}

public struct NameBlob
{
    public BlobArray<FixedString32Bytes> FirstNames;
    public BlobArray<FixedString32Bytes> LastNames;
}

public struct CleanupNameBlob : ICleanupComponentData
{
    public BlobAssetReference<NameBlob> NamesBlob;
}
