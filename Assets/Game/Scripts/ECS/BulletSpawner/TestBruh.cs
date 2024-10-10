using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;

public class TestBruh : MonoBehaviour
{

    private void Update()
    {
        // Debug.Log("Mono update");
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery eq = em.CreateEntityQuery(ComponentType.ReadOnly<Disabled>(), ComponentType.ReadWrite<BulletSpawnerComponent>());
        var a = eq.ToEntityArray(Allocator.Temp);
        Debug.Log(a.Length);
    }   
}