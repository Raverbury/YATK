using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using STG;
using MEC;
using System.Collections.Generic;

public class ECSEntitySpawner : MonoBehaviour
{
    private static EntityManager entityManager;
    private static EntityQuery getBulletSpawnerEntity;
    private static EntityQuery getInactiveEnemyBullets;
    private static EntityQuery getActiveEnemyBullets;
    private static EntityQuery getInactivePlayerBullets;
    private static EntityQuery getInactiveItems;
    private static Entity bulletPrefabEntity;

    private bool shouldCreateEntities = true;

    // private static EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Persistent);

    private void OnEnable()
    {
        StageManager.ClearEnemyBullet += ClearActiveEnemyBullets;
    }

    private void OnDisable()
    {
        StageManager.ClearEnemyBullet -= ClearActiveEnemyBullets;
    }

    private void ClearActiveEnemyBullets(bool shouldDropStarItem, bool forceClear)
    {
        var activeBullets = getActiveEnemyBullets.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < activeBullets.Length; i++)
        {
            Entity entity = activeBullets[i];
            var enemyBulletComponent = entityManager.GetComponentData<EnemyBulletComponent>(entity);
            if (!enemyBulletComponent.CanClear() && !forceClear)
            {
                continue;
            }
            DespawnEntity(activeBullets[i]);
            if (shouldDropStarItem)
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
                SpawnItemI1(bulletTransform.Position.x, bulletTransform.Position.y, ItemType.STAR_ITEM, true);
            }
        }
        activeBullets.Dispose();
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        getBulletSpawnerEntity = entityManager.CreateEntityQuery(ComponentType.ReadOnly<EntitySpawnerComponent>());

        EntityQueryDesc inactiveEnemyBulletsQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<Disabled>(), typeof(EnemyBulletComponent) },
        };
        getInactiveEnemyBullets = entityManager.CreateEntityQuery(inactiveEnemyBulletsQueryDesc);

        EntityQueryDesc activeEnemyBulletsQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(EnemyBulletComponent) },
            None = new ComponentType[] { typeof(Disabled) },
        };
        getActiveEnemyBullets = entityManager.CreateEntityQuery(activeEnemyBulletsQueryDesc);

        EntityQueryDesc inactivePlayerBulletsQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<Disabled>(), typeof(PlayerBulletComponent) },
        };
        getInactivePlayerBullets = entityManager.CreateEntityQuery(inactivePlayerBulletsQueryDesc);

        EntityQueryDesc inactiveItemsQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<Disabled>(), typeof(ItemComponent) },
        };
        getInactiveItems = entityManager.CreateEntityQuery(inactiveItemsQueryDesc);
    }

    private void Update()
    {
        if (shouldCreateEntities)
        {
            NativeArray<Entity> spawnerEntities = getBulletSpawnerEntity.ToEntityArray(Allocator.Temp);
            if (spawnerEntities.Length == 1)
            {
                Entity spawnerEntity = spawnerEntities[0];
                EntitySpawnerComponent bsc = entityManager.GetComponentData<EntitySpawnerComponent>(spawnerEntity);
                entityManager.SetName(spawnerEntity, "SpawnerEntity");
                if (!bsc.HasInitialized)
                {
                    bulletPrefabEntity = bsc.BulletPrefab;

                    ComponentTypeSet enemyBulletCTS = GetEnemyBulletComponentTypeSet();
                    ComponentTypeSet playerBulletCTS = GetPlayerBulletComponentTypeSet();
                    ComponentTypeSet itemCTS = GetItemComponentTypeSet();

                    // creating enemy bullet entities
                    for (int i = 0; i < bsc.AmountOfEnemyBulletsToAdd; i++)
                    {
                        Entity bulletEntity = entityManager.Instantiate(bsc.BulletPrefab);
                        entityManager.AddComponent(bulletEntity, enemyBulletCTS);
                        entityManager.SetName(bulletEntity, "EnemyBullet#" + bulletEntity.Index);
                        entityManager.GetComponentObject<SpriteRenderer>(bulletEntity).sortingOrder = 62;
                    }

                    // creating player bullet entities
                    for (int i = 0; i < bsc.AmountOfPlayerBulletsToAdd; i++)
                    {
                        Entity bulletEntity = entityManager.Instantiate(bsc.BulletPrefab);
                        entityManager.AddComponent(bulletEntity, playerBulletCTS);
                        entityManager.SetName(bulletEntity, "PlayerBullet#" + bulletEntity.Index);
                        entityManager.GetComponentObject<SpriteRenderer>(bulletEntity).sortingOrder = 20;
                    }

                    // creating item entities
                    for (int i = 0; i < bsc.AmountOfItemsToAdd; i++)
                    {
                        Entity itemEntity = entityManager.Instantiate(bsc.BulletPrefab);
                        entityManager.AddComponent(itemEntity, itemCTS);
                        entityManager.SetName(itemEntity, "Item#" + itemEntity.Index);
                        entityManager.GetComponentObject<SpriteRenderer>(itemEntity).sortingOrder = 50;
                    }

                    bsc.HasInitialized = true;
                    shouldCreateEntities = false;
                    entityManager.SetComponentData(spawnerEntity, bsc);
                }
                spawnerEntities.Dispose();
                return;
            }
            spawnerEntities.Dispose();
        }
    }

    // private void LateUpdate()
    // {
    //     if (entityCommandBuffer.ShouldPlayback)
    //     {
    //         entityCommandBuffer.Playback(entityManager);
    //         entityCommandBuffer.Dispose();
    //         entityCommandBuffer = new EntityCommandBuffer(Allocator.Persistent);
    //     }
    // }

    private static ComponentTypeSet GetEnemyBulletComponentTypeSet()
    {
        return new ComponentTypeSet(new[]{
            ComponentType.ReadWrite<BulletComponent>(),
            ComponentType.ReadWrite<EnemyBulletComponent>(),
            ComponentType.ReadWrite<EntityHitboxComponent>(),
            ComponentType.ReadOnly<Disabled>(),
        });
    }

    private static ComponentTypeSet GetPlayerBulletComponentTypeSet()
    {
        return new ComponentTypeSet(new[]{
            ComponentType.ReadWrite<BulletComponent>(),
            ComponentType.ReadWrite<PlayerBulletComponent>(),
            ComponentType.ReadOnly<Disabled>(),
        });
    }

    private static ComponentTypeSet GetItemComponentTypeSet()
    {
        return new ComponentTypeSet(new[]{
            ComponentType.ReadWrite<ItemComponent>(),
            ComponentType.ReadOnly<Disabled>(),
        });
    }

    #region generic bullet

    public static bool Exists(Entity entity)
    {
        return entityManager.Exists(entity);
    }

    /// <summary>
    /// Sets the speed of the entity representing a bullet
    /// </summary>
    /// <param name="entity"></param>
    public static void SetBulletSpeed(Entity entity, float speed)
    {
        BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
        bulletComponent.Speed = speed;
        // entityCommandBuffer.SetComponent(entity, bulletComponent);
        entityManager.SetComponentData(entity, bulletComponent);
    }

    /// <summary>
    /// Sets the facing angle (in degrees) so that the bullet's right angle or x+ vector points to this angle (sets euler Z rotation in under the hood)
    /// </summary>
    /// <param name="entity"></param>
    public static void SetBulletFacing(Entity entity, float facingAngle)
    {
        LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
        bulletTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, math.radians(facingAngle)));
        entityManager.SetComponentData(entity, bulletTransform);
    }

    /// <summary>
    /// Manipulate components such that the bullet entity is considered inactive.
    /// Involves adding Disabled to turn off renderer hide from active query and to make visible to inactive query
    /// </summary>
    /// <param name="entity"></param>
    public static void DespawnEntity(Entity entity)
    {
        CoroutineUtil.KillEntityBoundCoroutines(entity);
        entityManager.AddComponent(entity, ComponentType.ReadOnly<Disabled>());
    }

    /// <summary>
    /// Checks whether this entity has a Disabled component
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static bool EntityIsDisabled(Entity entity)
    {
        return entityManager.HasComponent<Disabled>(entity);
    }
    #endregion

    #region enemy bullet
    /// <summary>
    /// An overload that accepts a Vector2 as position instead of individual x and y
    /// </summary>
    /// <param name="position"></param>
    /// <param name="speed"></param>
    /// <param name="angle"></param>
    /// <param name="bulletType"></param>
    /// <param name="delay"></param>
    /// <param name="clearable"></param>
    /// <returns></returns>
    public static Entity SpawnEnemyBulletE1(Vector2 position, float speed, float angle, EnemyBulletType bulletType, int delay, bool clearable = true)
    {
        return SpawnEnemyBulletE1(position.x, position.y, speed, angle, bulletType, delay, clearable);
    }

    /// <summary>
    /// Spawns an enemy bullet with the given information. Angle is in degrees
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="speed"></param>
    /// <param name="angle"></param>
    /// <param name="bulletType"></param>
    /// <param name="delay"></param>
    /// <param name="clearable"></param>
    /// <returns></returns>
    public static Entity SpawnEnemyBulletE1(float x, float y, float speed, float angle, EnemyBulletType bulletType, int delay, bool clearable = true)
    {
        NativeArray<Entity> entities = getInactiveEnemyBullets.ToEntityArray(Allocator.Temp);
        Entity entity;
        // get first inactive bullet
        // if somehow there's no inactive bullet
        // then create 1 from scratch, shouldn't ever happen tbh
        if (entities.Length == 0)
        {
            Debug.Log("Creating new enemy bullet on the fly here!");
            entity = entityManager.Instantiate(bulletPrefabEntity);
            ComponentTypeSet enemyBulletCTS = GetEnemyBulletComponentTypeSet();
            entityManager.AddComponent(entity, enemyBulletCTS);
        }
        else
        {
            entity = entities[0];
        }
        // set speed to 0
        BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
        bulletComponent.Speed = speed;
        bulletComponent.ShouldMove = false;
        entityManager.SetComponentData(entity, bulletComponent);

        // set pos and angle
        LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
        bulletTransform.Position = new float3(x, y, 0f);
        bulletTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, math.radians(angle)));
        entityManager.SetComponentData(entity, bulletTransform);

        // set spawn cloud sprite
        (Sprite realSprite, float _1, float realHitboxRadius, Sprite spawnCloudSprite, float _2, float _3) = ShotSheet.GetEnemyBulletData(bulletType);
        SpriteRenderer spriteRenderer = entityManager.GetComponentObject<SpriteRenderer>(entity);
        spriteRenderer.sprite = spawnCloudSprite;

        // set hitbox radius to 0 so collision calc is skipped, so no hit/graze during spawn cloud phase
        var hitboxComponent = entityManager.GetSharedComponent<EntityHitboxComponent>(entity);
        hitboxComponent.CircleHitboxRadius = 0f;
        entityManager.SetSharedComponent(entity, hitboxComponent);

        // remove disabled to render and hide from future inactive queries
        entityManager.RemoveComponent<Disabled>(entity);

        // set flags for bullet
        var enemyBulletComponent = entityManager.GetComponentData<EnemyBulletComponent>(entity);
        enemyBulletComponent.SetBits(false, clearable);
        entityManager.SetComponentData(entity, enemyBulletComponent);

        // start counting down to spawn
        Timing.RunCoroutine(_SpawnEnemyBulletWithDelay(entity, speed, realSprite, realHitboxRadius, delay), "enemyBulletSpawning");

        // cleanup nativearray
        entities.Dispose();

        return entity;
    }

    /// <summary>
    /// MEC coroutine to spawn an enemy bullet after waiting for delay.
    /// Prior to this, the pos/rot, spawn cloud and 0 radius should have already been set by SpawnBulletA1
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="speed"></param>
    /// <param name="realSprite"></param>
    /// <param name="realHitboxRadius"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private static IEnumerator<float> _SpawnEnemyBulletWithDelay(Entity entity, float speed, Sprite realSprite, float realHitboxRadius, int delay)
    {
        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        SpawnEnemyBullet(entity, realHitboxRadius, realSprite);
    }

    /// <summary>
    /// Clears an enemy bullet.
    /// Calls DespawnBullet under the hood if this bullet is clearable only
    /// </summary>
    /// <param name="entity"></param>
    public static void ClearEnemyBullet(Entity entity)
    {
        var enemyBulletComponent = entityManager.GetComponentData<EnemyBulletComponent>(entity);
        if (enemyBulletComponent.CanClear())
        {
            DespawnEntity(entity);
        }
    }

    /// <summary>
    /// Clears a bullet.
    /// Calls DespawnBullet under the hood. Usable for all entities, and performs and additional clearable check if it has an EnemyBulletComponent
    /// </summary>
    /// <param name="entity"></param>
    public static void ClearBullet(Entity entity)
    {
        if (entityManager.HasComponent<EnemyBulletComponent>(entity))
        {
            var enemyBulletComponent = entityManager.GetComponentData<EnemyBulletComponent>(entity);
            if (!enemyBulletComponent.CanClear())
            {
                return;
            }
        }
        DespawnEntity(entity);
    }

    /// <summary>
    /// Manipulates components such that the bullet entity is set to spawn now after already waiting for spawn delay.
    /// Involves setting speed, sprite and hitbox to that of a real thing
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="speed"></param>
    /// <param name="radius"></param>
    /// <param name="sprite"></param>
    private static void SpawnEnemyBullet(Entity entity, float radius, Sprite sprite)
    {
        SpriteRenderer spriteRenderer = entityManager.GetComponentObject<SpriteRenderer>(entity);
        spriteRenderer.sprite = sprite;

        BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
        bulletComponent.ShouldMove = true;
        entityManager.SetComponentData(entity, bulletComponent);

        var hitboxComponent = entityManager.GetSharedComponent<EntityHitboxComponent>(entity);
        hitboxComponent.CircleHitboxRadius = radius;
        entityManager.SetSharedComponent(entity, hitboxComponent);
    }
    #endregion

    #region player bullet
    public static Entity SpawnPlayerBulletP1(float x, float y, float damage, float speed, float angle, PlayerShotType playerShotType, int delay)
    {
        NativeArray<Entity> entities = getInactivePlayerBullets.ToEntityArray(Allocator.Temp);
        Entity entity;
        // get first inactive bullet
        // if somehow there's no inactive bullet
        // then create 1 from scratch, shouldn't ever happen tbh
        if (entities.Length == 0)
        {
            Debug.Log("Creating new player bullet on the fly here!");
            entity = entityManager.Instantiate(bulletPrefabEntity);
            ComponentTypeSet playerBulletCTS = GetPlayerBulletComponentTypeSet();
            entityManager.AddComponent(entity, playerBulletCTS);
        }
        else
        {
            entity = entities[0];
        }
        // set speed to 0
        BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
        bulletComponent.Speed = 0;
        bulletComponent.ShouldMove = true;
        entityManager.SetComponentData(entity, bulletComponent);

        // set pos and angle
        LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
        bulletTransform.Position = new float3(x, y, 0f);
        bulletTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, math.radians(angle)));
        entityManager.SetComponentData(entity, bulletTransform);

        // set sprite
        ShotData playerShotData = ShotSheet.GetPlayerShotData(playerShotType);
        SpriteRenderer spriteRenderer = entityManager.GetComponentObject<SpriteRenderer>(entity);
        spriteRenderer.sprite = playerShotData.SPRITES[0];

        // set damage for bullet
        var playerBulletComponent = entityManager.GetComponentData<PlayerBulletComponent>(entity);
        playerBulletComponent.Damage = damage;
        entityManager.SetComponentData(entity, playerBulletComponent);

        // remove disabled to render and hide from future inactive queries
        entityManager.RemoveComponent<Disabled>(entity);

        // start counting down to spawn
        Timing.RunCoroutine(_SpawnPlayerBulletWithDelay(entity, speed, delay));

        // cleanup nativearray
        entities.Dispose();

        return entity;
    }

    private static IEnumerator<float> _SpawnPlayerBulletWithDelay(Entity entity, float speed, int delay)
    {
        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        SetBulletSpeed(entity, speed);
    }
    #endregion

    #region item
    public static void SpawnItemI1(GameObject gameObject, ItemType itemType, bool shouldAutoCollect = false, float initialYVelocity = 1.5f)
    {
        SpawnItemI1(gameObject.transform.position.x, gameObject.transform.position.y, itemType, shouldAutoCollect, initialYVelocity);
    }

    public static void SpawnItemI1(Vector2 pos, ItemType itemType, bool shouldAutoCollect = false, float initialYVelocity = 1.5f)
    {
        SpawnItemI1(pos.x, pos.y, itemType, shouldAutoCollect, initialYVelocity);
    }

    public static void SpawnItemI1(float x, float y, ItemType itemType, bool shouldAutoCollect = false, float initialYVelocity = 1.5f)
    {
        NativeArray<Entity> entities = getInactiveItems.ToEntityArray(Allocator.Temp);
        Entity entity;
        // get first inactive bullet
        // if somehow there's no inactive bullet
        // then create 1 from scratch, shouldn't ever happen tbh
        if (entities.Length == 0)
        {
            Debug.Log("Creating new item on the fly here!");
            entity = entityManager.Instantiate(bulletPrefabEntity);
            ComponentTypeSet itemCTS = GetItemComponentTypeSet();
            entityManager.AddComponent(entity, itemCTS);
        }
        else
        {
            entity = entities[0];
        }
        // set pos and angle
        LocalTransform itemTransform = entityManager.GetComponentData<LocalTransform>(entity);
        itemTransform.Position = new float3(x, y, 0f);
        itemTransform.Rotation = quaternion.EulerXYZ(new float3(0f, 0f, 0f));
        entityManager.SetComponentData(entity, itemTransform);

        // set speed and item type
        ItemComponent itemComponent = entityManager.GetSharedComponent<ItemComponent>(entity);
        itemComponent.SetNewItemType(itemType, shouldAutoCollect);
        itemComponent.Speed = initialYVelocity;
        entityManager.SetSharedComponent(entity, itemComponent);

        // set sprite
        SpriteRenderer spriteRenderer = entityManager.GetComponentObject<SpriteRenderer>(entity);
        spriteRenderer.sprite = ShotSheet.GetItemSprite(itemType);

        // remove disabled to render and hide from future inactive queries
        entityManager.RemoveComponent<Disabled>(entity);

        entities.Dispose();
    }
    #endregion
}