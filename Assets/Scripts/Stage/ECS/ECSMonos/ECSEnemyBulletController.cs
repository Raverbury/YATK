using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ECSEnemyBulletController : PausableMono
{
    private EntityManager entityManager;
    private EntityQuery activeEnemyBulletsQuery;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        activeEnemyBulletsQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<EnemyBulletComponent>());
    }

    protected override void PausableUpdate()
    {
        if (Player.instance == null)
        {
            return;
        }
        Player player = Player.instance;
        float3 playerPos = Player.instance.transform.position;
        NativeArray<Entity> allActiveEnemyBullets = activeEnemyBulletsQuery.ToEntityArray(Allocator.Temp);
        if (allActiveEnemyBullets.Length == 0)
        {
            allActiveEnemyBullets.Dispose();
            return;
        }
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        for (int i = 0; i < allActiveEnemyBullets.Length; i++)
        {
            Entity entity = allActiveEnemyBullets[i];
            EnemyBulletComponent enemyBulletComponent = entityManager.GetComponentData<EnemyBulletComponent>(entity);

            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

            EntityHitboxComponent entityHitboxComponent = entityManager.GetSharedComponent<EntityHitboxComponent>(entity);

            if (entityHitboxComponent.CircleHitboxRadius == 0f)
            {
                continue;
            }

            float distance = math.distance(localTransform.Position, playerPos);
            float finalPlayerHitboxRadius = player.playerData.hitboxRadius.GetFinalStat();
            float finalPlayerGrazeboxRadius = player.playerData.grazeboxRadius.GetFinalStat();
            float bulletHixbox = entityHitboxComponent.CircleHitboxRadius;
            if (distance <= finalPlayerGrazeboxRadius + finalPlayerHitboxRadius + bulletHixbox)
            {
                // graze
                if (!enemyBulletComponent.HasGrazed())
                {
                    player.PlayerGraze();
                    enemyBulletComponent.SetBits(true, enemyBulletComponent.CanClear());
                }
                if (distance <= finalPlayerHitboxRadius + bulletHixbox)
                {
                    // hit
                    player.PlayerGetHit();
                    ECSEntitySpawner.ClearEnemyBullet(entity);
                }
            }
            ecb.SetComponent(entity, enemyBulletComponent);
        }
        ecb.Playback(entityManager);
        ecb.Dispose();
        allActiveEnemyBullets.Dispose();
    }
}