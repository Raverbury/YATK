using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class ECSPlayerBulletController : PausableMono
{
    private EntityManager entityManager;
    private EntityQuery activePlayerBulletsQuery;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        activePlayerBulletsQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerBulletComponent>());
    }

    protected override void PausableUpdate()
    {
        if (StageManager.instance == null)
        {
            return;
        }
        GameObject[] enemyGOs = StageManager.instance.GetTargetableEnemies();
        if (enemyGOs.Length == 0)
        {
            return;
        }
        NativeArray<Entity> allActivePlayerBullets = activePlayerBulletsQuery.ToEntityArray(Allocator.Temp);
        if (allActivePlayerBullets.Length == 0)
        {
            allActivePlayerBullets.Dispose();
            return;
        }
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        for (int i = 0; i < allActivePlayerBullets.Length; i++)
        {
            Entity entity = allActivePlayerBullets[i];
            PlayerBulletComponent playerBulletComponent = entityManager.GetComponentData<PlayerBulletComponent>(entity);

            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

            bool hitSomething = false;

            for (int j = 0; j < enemyGOs.Length; j++)
            {
                float3 enemyPos = enemyGOs[j].transform.position;

                float distance = math.distance(localTransform.Position, enemyPos);
                Enemy enemy = enemyGOs[j].GetComponent<Enemy>();
                // TODO: maybe add enemy hurtbox radius here kappa
                if (distance <= (enemy.IsBoss ? 50f : 20f))
                {
                    enemyGOs[j].GetComponent<Enemy>().TakeDamage(playerBulletComponent.Damage);
                    hitSomething = true;
                }
            }
            if (hitSomething)
            {
                ECSEntitySpawner.DespawnEntity(entity);
            }
        }
        ecb.Playback(entityManager);
        ecb.Dispose();
        allActivePlayerBullets.Dispose();
    }
}