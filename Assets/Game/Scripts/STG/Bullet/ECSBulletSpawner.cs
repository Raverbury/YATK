using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using STG;
using MEC;
using System.Collections.Generic;

public class ECSBulletSpawner : MonoBehaviour
{
    private static EntityManager entityManager;
    private static EntityQuery getBulletSpawnerEntity;
    private static EntityQuery getInactiveBullets;
    private static EntityQuery getActiveBullets;
    private static Entity bulletPrefabEntity;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        getBulletSpawnerEntity = entityManager.CreateEntityQuery(ComponentType.ReadOnly<BulletSpawnerComponent>());
        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<Disabled>(), typeof(BulletComponent) },
        };
        getInactiveBullets = entityManager.CreateEntityQuery(desc);
        getActiveBullets = entityManager.CreateEntityQuery(ComponentType.ReadOnly<BulletSpawnerComponent>());
    }

    private void Update()
    {
        NativeArray<Entity> spawnerEntities = getBulletSpawnerEntity.ToEntityArray(Allocator.Temp);
        if (spawnerEntities.Length == 1)
        {
            Entity spawnerEntity = spawnerEntities[0];
            BulletSpawnerComponent bsc = entityManager.GetComponentData<BulletSpawnerComponent>(spawnerEntity);
            entityManager.SetName(spawnerEntity, "SpawnerEntity");
            if (!bsc.HasInitialized)
            {
                bulletPrefabEntity = bsc.BulletPrefab;
                NativeArray<ComponentType> a = entityManager.GetComponentTypes(bulletPrefabEntity, Allocator.Temp);
                foreach (var comp in a)
                {
                    Debug.Log(comp.GetType());
                }
                a.Dispose();
                for (int i = 0; i < bsc.AmountToAdd; i++)
                {
                    Entity bulletEntity = entityManager.Instantiate(bsc.BulletPrefab);
                    entityManager.AddComponentData(bulletEntity, new BulletComponent
                    {
                        Speed = 0f,
                    });
                    entityManager.AddComponent(bulletEntity, typeof(Disabled));

                    LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);
                    bulletTransform.Position = new float3(192f, 100f, 0f);
                    bulletTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, math.radians(270)));

                    entityManager.SetComponentData(bulletEntity, bulletTransform);
                }
                bsc.HasInitialized = true;
                entityManager.SetComponentData(spawnerEntity, bsc);
            }
        }
        spawnerEntities.Dispose();
    }

    public static Entity SpawnBulletA1(float x, float y, float speed, float angle, EnemyBulletType bulletType, int delay, bool clearable = true)
    {
        NativeArray<Entity> entities = getInactiveBullets.ToEntityArray(Allocator.Temp);
        Entity entity;
        if (entities.Length == 0)
        {
            Debug.Log("uh oh");
            entity = entityManager.Instantiate(bulletPrefabEntity);
            entityManager.AddComponentData(entity, new BulletComponent
            {
                Speed = 0f,
            });
            entityManager.AddComponent(entity, typeof(Disabled));
        }
        else
        {
            Debug.Log("all good");
            entity = entities[0];
        }
        LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
        bulletTransform.Position = new float3(x, y, 0f);
        bulletTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, math.radians(angle)));
        entityManager.SetComponentData(entity, bulletTransform);

        (Sprite realSprite, float realSize, float realHitboxRadius, Sprite spawnCloudSprite, float spawnCloudSize, float spawnCloudHitboxRadius) = ShotSheet.GetEnemyBulletData((int)bulletType);
        SpriteRenderer spriteRenderer = entityManager.GetComponentObject<SpriteRenderer>(entity);
        spriteRenderer.sprite = spawnCloudSprite;
        // enemyBullet.SetGraphic(spawnCloudSprite, spawnCloudSize, spawnCloudHitboxRadius);
        entityManager.RemoveComponent<Disabled>(entity);
        Timing.RunCoroutine(_SpawnBulletWithDelay(entity, speed, realSprite, realSize, realHitboxRadius, delay), "enemyBulletSpawning");
        entities.Dispose();
        return entity;
    }

    private static IEnumerator<float> _SpawnBulletWithDelay(Entity entity, float speed, Sprite realSprite, float realSize, float realHitboxRadius, int delay)
    {
        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        SpriteRenderer spriteRenderer = entityManager.GetComponentObject<SpriteRenderer>(entity);
        spriteRenderer.sprite = realSprite;

        BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
        bulletComponent.Speed = speed;
        entityManager.SetComponentData(entity, bulletComponent);
        // also add EnemyBulletComponent here?
    }
}