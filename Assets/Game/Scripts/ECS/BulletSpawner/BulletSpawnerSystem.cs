using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;

public partial struct BulletSpawnerSystem : ISystem
{
    private void OnUpdate(ref SystemState systemState)
    {
        EntityManager entityManager;
        Entity bulletSpawnerEntity;
        BulletSpawnerComponent bulletSpawnerComponent;

        try
        {
            entityManager = systemState.EntityManager;
            bulletSpawnerEntity = SystemAPI.GetSingletonEntity<BulletSpawnerComponent>();
            bulletSpawnerComponent = entityManager.GetComponentData<BulletSpawnerComponent>(bulletSpawnerEntity);
        }
        catch (Exception _)
        {
            return;
        }

        if (!bulletSpawnerComponent.UseEcs)
        {
            return;
        }

        if (bulletSpawnerComponent.TimeBetweenShot <= 0)
        {
            for (int i = 0; i < bulletSpawnerComponent.Branches; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = entityManager.Instantiate(bulletSpawnerComponent.BulletPrefab);
#if UNITY_EDITOR
                entityManager.SetName(bulletEntity, "Generated bullet");
#endif
                ECB.AddComponent(bulletEntity, new BulletComponent
                {
                    Speed = 2f,
                    FramesToLive = 60,
                });

                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);

                bulletTransform.Position = new Unity.Mathematics.float3(192f, -224f, 0f);
                bulletTransform.Rotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0f, 0f, 360f / bulletSpawnerComponent.Branches * i));

                ECB.SetComponent(bulletEntity, bulletTransform);

                ECB.Playback(entityManager);

                ECB.Dispose();
            }
            bulletSpawnerComponent.TimeBetweenShot = bulletSpawnerComponent.ShotInterval;
            entityManager.SetComponentData(bulletSpawnerEntity, bulletSpawnerComponent);
            return;
        }
        bulletSpawnerComponent.TimeBetweenShot -= 1;
        entityManager.SetComponentData(bulletSpawnerEntity, bulletSpawnerComponent);
    }
}