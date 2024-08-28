using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool
{
    private readonly List<GameObject> pool;
    private int currentIndex = 0;

    private readonly int capacity;

    public BulletPool(GameObject baseObj, int capacity)
    {
        this.capacity = capacity;
        pool = new List<GameObject>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            GameObject clone = Object.Instantiate(baseObj);
            clone.SetActive(false);
            pool.Add(clone);
        }
    }

    private GameObject RequestBullet()
    {
        return pool[GetNextInteger()];
    }

    private int GetNextInteger()
    {
        currentIndex = ((currentIndex + 1) >= capacity) ? 0 : (currentIndex + 1);
        return currentIndex;
    }

    public GameObject SpawnBulletA1(float x, float y, float speed, float angle, float delay)
    {
        GameObject bullet = RequestBullet();
        bullet.transform.position = new Vector3(x, y, 0);
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);
        bullet.TryGetComponent<EnemyBullet>(out EnemyBullet enemyBullet);
        enemyBullet.speed = speed;
        GlobalCoroutine.Start(SpawnBulletWithDelay(bullet, delay));
        return bullet;
    }

    IEnumerator SpawnBulletWithDelay(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        bullet.SetActive(true);
    }
}
