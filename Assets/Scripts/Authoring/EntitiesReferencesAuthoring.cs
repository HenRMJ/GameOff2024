using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject cultistPrefab;

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

                // Add the component with the BlobAssetReference
                AddComponent(entity, new EntitiesReferences
                {
                    CultistEntity = GetEntity(authoring.cultistPrefab, TransformUsageFlags.Dynamic),
                    NamesBlob = namesBlob,
                    Random = new Random((uint)System.Environment.TickCount)
                });
            }
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity CultistEntity;
    public BlobAssetReference<NameBlob> NamesBlob;
    public int RandomSeed;
    public Unity.Mathematics.Random Random;
}

public struct NameBlob
{
    public BlobArray<FixedString32Bytes> FirstNames;
    public BlobArray<FixedString32Bytes> LastNames;
}
