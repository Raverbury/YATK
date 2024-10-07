using System.Collections.Generic;
using MEC;
using STG;
using UnityEngine;

public class EnemyBulletPool : ObjectPool
{
    private static EnemyBulletPool instance = null;

    protected override void AltAwake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public static GameObject SpawnBulletA1(GameObject gameObject, float speed, float angle, EnemyBulletType bulletType, int delay, bool clearable = true)
    {
        return SpawnBulletA1(gameObject.transform.position.x, gameObject.transform.position.y, speed, angle, bulletType, delay, clearable);
    }

    public static GameObject SpawnBulletA1(Vector2 position, float speed, float angle, EnemyBulletType bulletType, int delay, bool clearable = true)
    {
        return SpawnBulletA1(position.x, position.y, speed, angle, bulletType, delay, clearable);
    }

    public static GameObject SpawnBulletA1(float x, float y, float speed, float angle, EnemyBulletType bulletType, int delay, bool clearable = true)
    {
        GameObject bullet = instance.RequestObject();
        bullet.transform.position = new Vector3(x, y, 0);
        bullet.transform.eulerAngles = new Vector3(0, 0, angle);
        bullet.TryGetComponent(out EnemyBullet enemyBullet);
        enemyBullet.speed = speed;
        enemyBullet.SetClearable(clearable);
        (Sprite realSprite, float realSize, float realHitboxRadius, Sprite spawnCloudSprite, float spawnCloudSize, float spawnCloudHitboxRadius) = ShotSheet.GetEnemyBulletData((int)bulletType);
        enemyBullet.SetGraphic(spawnCloudSprite, spawnCloudSize, spawnCloudHitboxRadius);
        Timing.RunCoroutine(_SpawnBulletWithDelay(bullet, enemyBullet, realSprite, realSize, realHitboxRadius, delay), "enemyBulletSpawning");
        return bullet;
    }

    private static IEnumerator<float> _SpawnBulletWithDelay(GameObject bullet, EnemyBullet enemyBullet, Sprite realSprite, float realSize, float realHitboxRadius, int delay)
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