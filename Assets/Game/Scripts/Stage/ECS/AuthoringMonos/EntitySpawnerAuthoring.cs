using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public class EntitySpawnerAuthoring : MonoBehaviour
{
    [Tooltip("The GO used to instantiate all player bullets, enemy bullets and items as entities. Should have a SpriteRenderer attached minimum and nothing else. Scales should also be set to 100 for all 3 axes.")]
    public GameObject GenericEntityPrefab;

    public int AmountOfEnemyBulletsToAdd = 500;
    public int AmountOfPlayerBulletsToAdd = 500;
    public int AmountOfItemsToAdd = 500;

    public static Entity entity;

    public class BulletSpawnerBaker : Baker<EntitySpawnerAuthoring>
    {
        public override void Bake(EntitySpawnerAuthoring authoring)
        {
            Entity spawnerEntity = GetEntity(TransformUsageFlags.None);
            entity = spawnerEntity;
            AddComponent(spawnerEntity, new EntitySpawnerComponent
            {
                BulletPrefab = GetEntity(authoring.GenericEntityPrefab, TransformUsageFlags.None),
                AmountOfEnemyBulletsToAdd = authoring.AmountOfEnemyBulletsToAdd,
                AmountOfPlayerBulletsToAdd = authoring.AmountOfPlayerBulletsToAdd,
                AmountOfItemsToAdd = authoring.AmountOfItemsToAdd,
                HasInitialized = false,
            });
        }
    }
}