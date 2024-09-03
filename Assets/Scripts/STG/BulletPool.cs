using System.Collections.Generic;
using MEC;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField]
    private GameObject baseObj;
    [SerializeField]
    private int capacity;

    private static BulletPool instance = null;
    private List<GameObject> pool;
    private int currentIndex = 0;

    private void Awake()
    {
        if (instance != null) {
            Destroy(this);
        }
        instance = this;
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

    public static GameObject SpawnBulletA1(float x, float y, float speed, float angle, ShotData graphic, int delay)
    {
        GameObject bullet = instance.RequestBullet();
        bullet.transform.position = new Vector3(x, y, 0);
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);
        bullet.TryGetComponent(out EnemyBullet enemyBullet);
        enemyBullet.speed = speed;
        enemyBullet.SetGraphic(graphic);
        Timing.RunCoroutine(_SpawnBulletWithDelay(bullet, delay));
        return bullet;
    }

    static IEnumerator<float> _SpawnBulletWithDelay(GameObject bullet, int delay)
    {
        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        bullet.SetActive(true);
    }
}
