using Unity.Collections;
using Unity.Entities;
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
        NativeArray<Entity> allEntities = entityQuery.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < allEntities.Length; i++)
        {
            Entity entity = allEntities[i];
            if (entityManager.HasComponent<BulletComponent>(entity))
            {
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);

                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

                localTransform.Position += bulletComponent.Speed * localTransform.Right();

                entityManager.SetComponentData(entity, localTransform);
            }
        }
        allEntities.Dispose();
    }
}