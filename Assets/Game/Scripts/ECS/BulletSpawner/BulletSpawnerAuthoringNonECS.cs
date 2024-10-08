using Unity.Entities;
using UnityEngine;

public class BulletSpawnerAuthoringNonECS : MonoBehaviour
{
    public GameObject BulletPrefab;
    private EntityManager entityManager;

    private int timeBetweenShot = 0;
    public int ShotInterval = 1;

    public int Branch = 20;

    private void Start() {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update() {
        Debug.Log("Hello");
        // if (timeBetweenShot <= 0) {
        //     for (int i = 0; i < Branch; i++)
        //     {
        //         Entity bulletEntity = entityManager.Instantiate(entityManager.);
        //         entityManager.AddComponent(bulletEntity, new BulletComponent{
        //             Speed = 2f,
        //             FramesToLive = 60,
        //         });
        //     }
        //     timeBetweenShot = ShotInterval;
        //     return;
        // }
        // timeBetweenShot -= 1;
    }
}