using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial struct BulletSpawnerSystem : ISystem
{

    // private void OnUpdate(ref SystemState systemState)
    // {
    //     if (SceneManager.GetActiveScene().name != "ECS")
    //     {
    //         return;
    //     }
    //     EntityManager entityManager = systemState.EntityManager;
    //     if (SystemAPI.TryGetSingletonEntity<BulletSpawnerComponent>(out Entity spawnerEntity))
    //     {
    //         BulletSpawnerComponent bsc = entityManager.GetComponentData<BulletSpawnerComponent>(spawnerEntity);
    //         entityManager.SetName(spawnerEntity, "SpawnerEntity");
    //         if (!bsc.HasInitialized)
    //         {
    //             for (int i = 0; i < bsc.AmountToAdd; i++)
    //             {
    //                 Entity bulletEntity = entityManager.Instantiate(bsc.BulletPrefab);
    //                 entityManager.AddComponentData(bulletEntity, new BulletComponent
    //                 {
    //                     Speed = 0f,
    //                 });

    //                 LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);

    //                 UnityEngine.Vector2 v = new UnityEngine.Vector2(192, -224) + UnityEngine.Random.insideUnitCircle * 120f;
    //                 bulletTransform.Position = new Unity.Mathematics.float3(v.x, v.y, 0f);
    //                 bulletTransform.Rotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0f, 0f, 270f));

    //                 entityManager.SetComponentData(bulletEntity, bulletTransform);
    //             }
    //             bsc.HasInitialized = true;
    //             entityManager.SetComponentData(spawnerEntity, bsc);
    //         }
    //         else
    //         {
    //             if (!bsc.DoMath)
    //             {
    //                 return;
    //             }
    //             NativeArray<Entity> allEntities = entityManager.GetAllEntities();
    //             for (int i = 0; i < allEntities.Length; i++)
    //             {
    //                 Entity entity = allEntities[i];
    //                 if (entityManager.HasComponent<BulletComponent>(entity))
    //                 {
    //                     LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

    //                     math.distance(localTransform.Position, new float3(192f, -360f, 0f));
    //                 }
    //             }
    //             allEntities.Dispose();
    //         }
    //     }
    // }

    // private void ReceiveCandy(int idk)
    // {
    //     Debug.Log(idk);
    //     // SceneUtil.LoadSceneAsync("Home");
    // }
}
