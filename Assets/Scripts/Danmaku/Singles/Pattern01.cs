using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Pattern01 : MonoBehaviour
{
    private BulletPool pool;

    [SerializeField]
    private GameObject bullet;

    private void Awake()
    {
        pool = new BulletPool(bullet, 1000);
    }

    private void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        const int BRANCHES = 100;
        while (true)
        {
            yield return new WaitForSeconds(2);
            float r = Random.Range(0, 20);
            for (int i = 0; i < BRANCHES; i++) {
                GameObject bullet = pool.SpawnBulletA1(192, -60, 2, 360f / (BRANCHES / 2) * i + r, 0.5f);
                StartCoroutine(Manipulate(bullet, (i >= (BRANCHES / 2))? 1 : -1));
            }
            yield return null;
        }
    }

    IEnumerator Manipulate(GameObject bullet, int direction) {
        yield return new WaitForSeconds(1.2f);

        bullet.TryGetComponent<EnemyBullet>(out EnemyBullet enemyBullet);
        enemyBullet.rotationSpeed = 0.1f * direction;
    }
}
