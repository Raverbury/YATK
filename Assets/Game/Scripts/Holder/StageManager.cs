using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

[RequireComponent(typeof(ShotSheet))]
[RequireComponent(typeof(BulletPool))]
public class StageManager : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private BulletPool bulletPool;
    private static StageManager instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate()
    {
        bulletPool = GetComponent<BulletPool>();
    }

    public static GameObject SpawnBulletA1(float x, float y, float speed, float angle, EnemyBulletType bulletType, int delay)
    {
        GameObject bullet = instance.bulletPool.RequestBullet();
        bullet.transform.position = new Vector3(x, y, 0);
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);
        bullet.TryGetComponent(out EnemyBullet enemyBullet);
        enemyBullet.speed = speed;
        (Sprite realSprite, float realSize, float realHitboxRadius, Sprite spawnCloudSprite, float spawnCloudSize, float spawnCloudHitboxRadius) = ShotSheet.GetEnemyBulletData((int)bulletType);
        enemyBullet.SetGraphic(spawnCloudSprite, spawnCloudSize, spawnCloudHitboxRadius);
        Timing.RunCoroutine(_SpawnBulletWithDelay(bullet, enemyBullet, realSprite, realSize, realHitboxRadius, delay));
        return bullet;
    }

    static IEnumerator<float> _SpawnBulletWithDelay(GameObject bullet, EnemyBullet enemyBullet, Sprite realSprite, float realSize, float realHitboxRadius, int delay)
    {
        bullet.SetActive(true);
        enemyBullet.enabled = false;

        for (int __delay = 0; __delay < delay; __delay++)
        {
            yield return Timing.WaitForOneFrame;
        }

        enemyBullet.SetGraphic(realSprite, realSize, realHitboxRadius);
        enemyBullet.enabled = true;
        bullet.SetActive(true);
    }
}