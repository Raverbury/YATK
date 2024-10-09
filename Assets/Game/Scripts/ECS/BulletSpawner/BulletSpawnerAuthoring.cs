using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

[BurstCompile]
public class BulletSpawnerAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab;
    public int ShotInterval;
    public int Branches;

    public bool UseECS;

    private GameObject[] pool;
    private int timeBetweenShot = 0;

    private const int POOL_SIZE = 1000;
    private int internalPoolIndex = 0;

    private void Start()
    {
        if (UseECS)
        {
            return;
        }


        pool = new GameObject[1000];
        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameObject newBullet = Instantiate(BulletPrefab);
            newBullet.AddComponent(typeof(BruhBullet));
            pool[i] = newBullet;
        }
    }

    private void Update()
    {
        if (timeBetweenShot <= 0)
        {
            Vector3 pos = new Vector3(192f, -224f, 0f);
            for (int i = 0; i < Branches; i++)
            {
                Quaternion rotation = Quaternion.Euler(new UnityEngine.Vector3(0f, 0f, 360f / Branches * i));
                GameObject bullet = SpawnBullet(pos, rotation, 2, 60);
                bullet.SetActive(true);
            }
            timeBetweenShot = ShotInterval;
            return;
        }
        timeBetweenShot -= 1;
    }

    private GameObject SpawnBullet(Vector3 position, Quaternion rotation, float speed, int framesToLive)
    {
        GameObject result = pool[internalPoolIndex];
        BruhBullet bruhBullet = result.GetComponent<BruhBullet>();
        bruhBullet.Speed = speed;
        bruhBullet.FramesToLive = framesToLive;
        result.transform.position = position;
        result.transform.rotation = rotation;
        internalPoolIndex = (internalPoolIndex + 1) % POOL_SIZE;
        return result;
    }

    public class BulletSpawnerBaker : Baker<BulletSpawnerAuthoring>
    {
        public override void Bake(BulletSpawnerAuthoring authoring)
        {
            if (!authoring.UseECS)
            {
                return;
            }
            Entity spawnerEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(spawnerEntity, new BulletSpawnerComponent
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.None),
                TimeBetweenShot = 0,
                ShotInterval = authoring.ShotInterval,
                Branches = authoring.Branches,
                UseEcs = authoring.UseECS,
            });
        }
    }
}