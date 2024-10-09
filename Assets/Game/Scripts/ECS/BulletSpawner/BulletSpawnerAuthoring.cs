using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public class BulletSpawnerAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab;
    public GameObject[] bullets;

    public GameObject playerGO;

    public int AmountToAdd = 500;

    public bool UseECS = true;

    public bool DoMath = true;

    private void Start()
    {
        bullets = new GameObject[AmountToAdd];
        for (int i = 0; i < AmountToAdd; i++)
        {
            GameObject cloned = Instantiate(BulletPrefab);
            cloned.AddComponent<BruhBullet>();
            cloned.transform.position = new Vector2(192f, -334f) + Random.insideUnitCircle * 120f;
            bullets[i] = cloned;
        }
    }

    private void Update()
    {
        if (!DoMath)
        {
            return;
        }
        for (int i = 0; i < AmountToAdd; i++)
        {
            float dist = Vector2.Distance(new Vector2(192, -360), bullets[i].transform.position);
            if (dist < 0.1f)
            {
                break;
            }
        }
    }

    public class BulletSpawnerBaker : Baker<BulletSpawnerAuthoring>
    {
        public override void Bake(BulletSpawnerAuthoring authoring)
        {
            if (!authoring.UseECS)
            {
                Instantiate(authoring.gameObject);
                return;
            }
            Entity spawnerEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(spawnerEntity, new BulletSpawnerComponent
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.None),
                AmountToAdd = authoring.AmountToAdd,
                HasInitialized = false,
                DoMath = authoring.DoMath,
            });
        }
    }
}