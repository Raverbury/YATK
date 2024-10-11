using STG;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;

public class ECSBulletController : PausableMono
{
    private EntityManager entityManager;
    private EntityQuery entityQuery;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<BulletComponent>());
    }

    protected override void PausableUpdate()
    {
        NativeArray<Entity> allBullets = entityQuery.ToEntityArray(Allocator.Temp);
        if (allBullets.Length == 0)
        {
            return;
        }
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        for (int i = 0; i < allBullets.Length; i++)
        {
            Entity entity = allBullets[i];
            if (entityManager.HasComponent<BulletComponent>(entity))
            {
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
                if (!bulletComponent.ShouldMove) {
                    continue;
                }

                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

                localTransform.Position += bulletComponent.Speed * localTransform.Right();

                // check out of play area bounds
                if (localTransform.Position.y < Constant.GAME_BORDER_BOTTOM - 100 ||
                localTransform.Position.y > Constant.GAME_BORDER_TOP + 100)
                {
                    ECSEntitySpawner.DespawnEntity(entity);
                }
                else if (localTransform.Position.x < Constant.GAME_BORDER_LEFT - 100 ||
                localTransform.Position.x > Constant.GAME_BORDER_RIGHT + 100)
                {
                    ECSEntitySpawner.DespawnEntity(entity);
                }

                ecb.SetComponent(entity, localTransform);
            }
        }
        ecb.Playback(entityManager);
        ecb.Dispose();
        allBullets.Dispose();
    }
}