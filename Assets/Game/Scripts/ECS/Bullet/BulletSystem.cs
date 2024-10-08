using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    public readonly void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager; ;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();
        for (int i = 0; i < allEntities.Length; i++)
        {
            Entity entity = allEntities[i];
            if (entityManager.HasComponent<BulletComponent>(entity))
            {
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

                localTransform.Position += bulletComponent.Speed * localTransform.Right();

                entityManager.SetComponentData(entity, localTransform);

                bulletComponent.FramesToLive -= 1;
                if (bulletComponent.FramesToLive <= 0)
                {
                    entityManager.DestroyEntity(entity);
                    continue;
                }
                entityManager.SetComponentData(entity, bulletComponent);
            }
        }
    }
}