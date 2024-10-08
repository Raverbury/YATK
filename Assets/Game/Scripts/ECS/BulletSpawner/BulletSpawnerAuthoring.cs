using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public class BulletSpawnerAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab;
    public int ShotInterval;
    public int Branches;

    public class BulletSpawnerBaker : Baker<BulletSpawnerAuthoring>
    {
        public override void Bake(BulletSpawnerAuthoring authoring)
        {
            Entity spawnerEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(spawnerEntity, new BulletSpawnerComponent
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.None),
                TimeBetweenShot = 0,
                ShotInterval = authoring.ShotInterval,
                Branches = authoring.Branches,
            });
        }
    }
}