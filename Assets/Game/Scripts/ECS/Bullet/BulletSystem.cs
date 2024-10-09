using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct BulletSystem : ISystem
{
    public readonly void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager; ;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();
        for (int i = 0; i < allEntities.Length; i++)
        {
            Entity entity = allEntities[i];
            if (entityManager.HasComponent<BulletComponent>(entity))
            {
                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);
                if (localTransform.Position.y < -448)
                {
                    localTransform.Position += new Unity.Mathematics.float3(0f, 500f, 0f);
                    entityManager.GetComponentObject<SpriteRenderer>(entity).sprite = ShotSheet.GetItemSprite(STG.ItemType.POWER_ITEM);
                }

                localTransform.Position += 2f * localTransform.Right();

                entityManager.SetComponentData(entity, localTransform);
            }
        }
    }
}