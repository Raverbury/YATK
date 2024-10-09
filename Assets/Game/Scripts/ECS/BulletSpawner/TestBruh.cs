using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;

public class TestBruh : MonoBehaviour
{
    public static UnityAction<int> FreeCandy;
    public static UnityAction<bool> ECSReady;

    private int counter = 0;

    private void Start()
    {
        ECSReady?.Invoke(true);
    }

    private void OnDestroy()
    {
        ECSReady?.Invoke(false);
    }

    private void Update()
    {
        Debug.Log("Mono update");
        counter += 1;
        if (counter > 300 && UnityEngine.Random.Range(0f, 1f) <= 0.05f)
        {
            FreeCandy?.Invoke(UnityEngine.Random.Range(0, 100)); ;
        }
        // EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // NativeArray<Entity> allEntities = entityManager.GetAllEntities();
        // for (int i = 0; i < allEntities.Length; i++)
        // {
        //     Entity entity = allEntities[i];
        //     if (entityManager.HasComponent<BulletComponent>(entity))
        //     {
        //         LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

        //         math.distance(localTransform.Position, new float3(192f, -360f, 0f));
        //     }
        // }
    }
}